using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Storage;

namespace Speakeasy.Server.Models.Database.Repositories;

public class FileRepository : IFileRepository
{
    private readonly IFileStore _fileStore;
    private readonly DbSet<StoredFile> _db;

    public FileRepository(IFileStore fileStore, SpeakeasyDbContext context)
    {
        _fileStore = fileStore;
        
    }
    
    public async Task AddFileAsync(Stream stream, StoredFile file)
    {
        await _fileStore.WriteFileAsync(file.Id, stream);

        await _db.AddAsync(file);
    }

    public async Task<bool> ContainsFileAsync(Guid id)
    {
        return await _db.AsNoTracking().AnyAsync(e => e.Id == id);
    }

    public async Task<StoredFile?> GetFileByIdAsync(Guid id, bool trackEntity = true)
    {
        IQueryable<StoredFile> query = _db;

        if (trackEntity is false)
        {
            query = query.AsNoTracking();
        }
        
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Stream?> GetFileStreamByIdAsync(Guid id)
    {
        if (!await _fileStore.DoesFileExistAsync(id))
        {
            return null;
        }

        return await _fileStore.GetFileAsync(id);
    }

    public async Task DeleteFileById(Guid id)
    {
        var storedFile = await GetFileByIdAsync(id);
        var existsInFileStore = await _fileStore.DoesFileExistAsync(id);

        if (storedFile is null)
        {
            if (existsInFileStore)
            {
                await _fileStore.RemoveAsync(id);
            }

            return;
        }

        await _fileStore.RemoveAsync(id);
        _db.Remove(storedFile);
    }
}