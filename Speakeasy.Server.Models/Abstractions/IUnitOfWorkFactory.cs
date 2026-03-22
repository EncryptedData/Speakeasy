namespace Speakeasy.Server.Models.Abstractions;

public interface IUnitOfWorkFactory
{
    Task<IUnitOfWork> GetUnitOfWorkAsync(CancellationToken cancellationToken = default);
}