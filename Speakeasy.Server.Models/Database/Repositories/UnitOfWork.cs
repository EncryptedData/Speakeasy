using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Storage;

namespace Speakeasy.Server.Models.Database.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SpeakeasyDbContext _context;

    public UnitOfWork(SpeakeasyDbContext context, IFileStore fileStore)
    {
        _context = context;
        ChannelRepository = new ChannelRepository(_context);
        GroupRepository = new GroupRepository(_context);
        MessageRepository = new MessageRepository(_context);
        FileRepository = new FileRepository(fileStore, _context);
        UserRepository = new UserRepository(_context);
        GroupRoleRepository = new GroupRoleRepository(_context);
        CustomEmojiRepository = new CustomEmojiRepository(_context);
        GifRepository = new GifRepository(_context);
    }
    
    public IChannelRepository ChannelRepository { get; }
    
    public IGroupRepository GroupRepository { get; }
    
    public IMessageRepository MessageRepository { get; }
    
    public IFileRepository FileRepository { get; }

    public IUserRepository UserRepository { get; }
    
    public IGroupRoleRepository GroupRoleRepository { get; }
    
    public ICustomEmojiRepository CustomEmojiRepository { get; }
    
    public IGifRepository GifRepository { get; }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}