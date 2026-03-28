using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class MessageController : BaseRepositoryController<Message, MessageDto>
{
    public MessageController(
        IUnitOfWork uow,
        IModelConverter<Message, MessageDto> converter) : 
        base(uow.MessageRepository, converter, uow)
    {
    }

    public override Task<ActionResult<MessageDto>> PostAsync(MessageDto dto)
    {
        return Task.FromResult<ActionResult<MessageDto>>(NotFound());
    }
}