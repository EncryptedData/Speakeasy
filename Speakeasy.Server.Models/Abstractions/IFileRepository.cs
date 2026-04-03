using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IFileRepository
{
    Task AddFileAsync(Stream stream, StoredFile file);

    Task<bool> ContainsFileAsync(Guid id);

    Task<StoredFile?> GetFileByIdAsync(Guid id, bool trackEntity = true);
    
    Task<Stream?> GetFileStreamByIdAsync(Guid id);

    Task DeleteFileById(Guid id);
}