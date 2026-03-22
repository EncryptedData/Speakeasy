using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    public MessageRepository(SpeakeasyDbContext context) : 
        base(context.Messages)
    {
    }
}