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

    public override async Task<ActionResult> DeleteAsync(Guid id)
    {
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(id);

        if (message is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        if (message.IsDeleted)
        {
            return NoContent();
        }
        
        message.IsDeleted = true;
        await _unitOfWork.CommitAsync();
        
        return NoContent();
    }
}