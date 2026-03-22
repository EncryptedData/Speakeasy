using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SpeakeasyDbContext _context;

    public UnitOfWork(SpeakeasyDbContext context)
    {
        _context = context;
        ChannelRepository = new ChannelRepository(_context);
        GroupRepository = new GroupRepository(_context);
        MessageRepository = new MessageRepository(_context);
        UserRepository = new UserRepository(_context);
    }
    
    public IChannelRepository ChannelRepository { get; }
    
    public IGroupRepository GroupRepository { get; }
    
    public IMessageRepository MessageRepository { get; }
    
    public IUserRepository UserRepository { get; }
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}