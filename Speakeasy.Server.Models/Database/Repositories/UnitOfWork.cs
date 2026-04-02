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
    }
    
    public IChannelRepository ChannelRepository { get; }
    
    public IGroupRepository GroupRepository { get; }
    
    public IMessageRepository MessageRepository { get; }
    
    public IFileRepository FileRepository { get; set; }

    public IUserRepository UserRepository { get; set; }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}