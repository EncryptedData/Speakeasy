using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    IChannelRepository ChannelRepository { get; }
    
    IGroupRepository GroupRepository { get; }
    
    IMessageRepository MessageRepository { get; }
    
    IFileRepository FileRepository { get; set; }
    
    public IUserRepository UserRepository { get; set; }
    
    public IGroupRoleRepository GroupRoleRepository { get; set; }
    
    public IGroupRolePermissionRepository GroupRolePermissionRepository { get; set; }
    
    public IGroupRoleClaimRepository GroupRoleClaimRepository { get; set; }
    
    ICustomEmojiRepository CustomEmojiRepository { get; set; }

    Task CommitAsync(CancellationToken cancellationToken = default);
}