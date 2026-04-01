using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Storage;

namespace Speakeasy.Server.Models.Database.Repositories;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory<SpeakeasyDbContext> _contextFactory;
    private readonly IFileStore _fileStore;

    public UnitOfWorkFactory(IDbContextFactory<SpeakeasyDbContext> contextFactory, IFileStore fileStore)
    {
        _contextFactory = contextFactory;
        _fileStore = fileStore;
    }
    
    public async Task<IUnitOfWork> GetUnitOfWorkAsync(CancellationToken cancellationToken = default)
    {
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return new UnitOfWork(context, _fileStore);
    }
}