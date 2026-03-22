namespace Speakeasy.Server.Models.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    public IChannelRepository ChannelRepository { get; }
    
    public IGroupRepository GroupRepository { get; }
    
    public IMessageRepository MessageRepository { get; }
    
    public IUserRepository UserRepository { get; }

    public Task CommitAsync(CancellationToken cancellationToken = default);
}