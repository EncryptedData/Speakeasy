using System.IO.Abstractions;
using Speakeasy.Server.Storage.Options;

namespace Speakeasy.Server.Storage;

public class LocalFileStore : IFileStore
{
    public const string Key = nameof(LocalFileStore);
    
    private readonly IFileSystem _fileSystem;
    private readonly string _baseUploadPath;

    public LocalFileStore(IFileSystem fileSystem, StorageOptions options)
    {
        _fileSystem = fileSystem;
        _baseUploadPath = _fileSystem.Path.GetFullPath(options.LocalPath);
    }
    
    public async Task WriteFileAsync(Guid id, Stream stream)
    {
        string filePath = GetFilePath(id);

        await using var fileStream = _fileSystem.File.Create(filePath);
        await stream.CopyToAsync(fileStream);
    }

    public Task<bool> DoesFileExistAsync(Guid id)
    {
        var filePath = GetFilePath(id, createDirectory: false);

        return Task.FromResult(_fileSystem.File.Exists(filePath));
    }

    public Task<Stream> GetFileAsync(Guid id)
    {
        var filePath = GetFilePath(id, createDirectory: false);

        if (!_fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        return Task.FromResult<Stream>(_fileSystem.File.OpenRead(filePath));
    }

    private string GetFilePath(Guid id, bool createDirectory = true)
    {
        string idStr = id.ToString();

        string folder = _fileSystem.Path.Combine(_baseUploadPath, idStr[0].ToString(), idStr[1].ToString());

        if (createDirectory)
        {
            _fileSystem.Directory.CreateDirectory(folder);
        }

        return _fileSystem.Path.Combine(folder, idStr);
    }
}