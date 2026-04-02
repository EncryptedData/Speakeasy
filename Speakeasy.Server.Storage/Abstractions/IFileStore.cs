namespace Speakeasy.Server.Storage;

public interface IFileStore
{
    Task WriteFileAsync(Guid id, Stream stream);

    Task<bool> DoesFileExistAsync(Guid id);

    Task<Stream> GetFileAsync(Guid id);

    Task RemoveAsync(Guid id);
}