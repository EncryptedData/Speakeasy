using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory<SpeakeasyDbContext> _contextFactory;

    public UnitOfWorkFactory(IDbContextFactory<SpeakeasyDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<IUnitOfWork> GetUnitOfWorkAsync(CancellationToken cancellationToken = default)
    {
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return new UnitOfWork(context);
    }
}