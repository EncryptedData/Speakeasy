namespace Speakeasy.Server.Models.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    public IChannelRepository ChannelRepository { get; }
    
    public IGroupRepository GroupRepository { get; }
    
    public IMessageRepository MessageRepository { get; }
    
    public IFileRepository FileRepository { get; set; }
    
    public IUserRepository UserRepository { get; set; }

    public Task CommitAsync(CancellationToken cancellationToken = default);
}