using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    IChannelRepository ChannelRepository { get; }
    
    IGroupRepository GroupRepository { get; }
    
    IMessageRepository MessageRepository { get; }
    
    IFileRepository FileRepository { get; }
    
    IUserRepository UserRepository { get; }
    
    IGroupRoleRepository GroupRoleRepository { get; }
    
    ICustomEmojiRepository CustomEmojiRepository { get; }
    
    IGifRepository GifRepository { get; }

    Task CommitAsync(CancellationToken cancellationToken = default);
}