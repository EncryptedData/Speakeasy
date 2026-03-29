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