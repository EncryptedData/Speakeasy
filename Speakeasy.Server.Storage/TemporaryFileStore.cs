using System.IO.Abstractions;
using Speakeasy.Server.Storage.Abstractions;

namespace Speakeasy.Server.Storage;

public class TemporaryFileStore : ITemporaryFileStore
{
    private readonly IFileSystem _fileSystem;

    public TemporaryFileStore(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    
    public ITemporaryFile CreateTemporaryFile()
    {
        var temporaryFile = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString());
        
        return new TemporaryFile(temporaryFile, _fileSystem);
    }
}