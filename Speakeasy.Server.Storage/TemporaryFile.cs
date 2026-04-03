using System.IO.Abstractions;
using Speakeasy.Server.Storage.Abstractions;

namespace Speakeasy.Server.Storage;

public class TemporaryFile : ITemporaryFile
{
    private readonly string _file;
    private readonly FileStream _stream;
    private readonly IFileSystem _fileSystem;

    public TemporaryFile(string file, IFileSystem fileSystem)
    {
        _file = file;
        _stream = new FileStream(_file, FileMode.Create);
        _fileSystem = fileSystem;
    }
    
    public Stream GetStream()
    {
        return _stream;
    }
    
    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();

        _fileSystem.File.Delete(_file);
    }
}