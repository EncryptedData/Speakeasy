using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Controllers.ApiVersion1.Hubs;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class MessageController : BaseRepositoryController<Message, MessageDto>
{
    private readonly ISpeakeasyV1HubService _hubService;

    public MessageController(
        IUnitOfWork uow,
        IModelConverter<Message, MessageDto> converter,
        ISpeakeasyV1HubService hubService) : 
        base(uow.MessageRepository, converter, uow)
    {
        _hubService = hubService;
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
        
    protected override async Task OnEntityCreatedAsync(MessageDto dto)
    {
        await _hubService.SendNotificationToAllAsync(e => e.MessageReceivedAsync(dto.ChannelId!.Value, dto));
    }

    protected override Task OnEntityUpdatedAsync(Guid id)
    {
        // Message edited?
        return Task.CompletedTask;
    }

    protected override Task OnEntityDeletedAsync(Guid id)
    {
        // Message deleted by sender?
        return Task.CompletedTask;
    }
}