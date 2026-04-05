using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Controllers.ApiVersion1.Hubs;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class GroupController : BaseRepositoryController<Group, GroupDto>
{
    private readonly IModelConverter<Channel, ChannelDto> _channelConverter;
    private readonly IModelConverter<CustomEmoji, CustomEmojiDto> _emojiConverter;
    private readonly ISpeakeasyV1HubService _hubService;
    
    public GroupController(
        IUnitOfWork uow, 
        IModelConverter<Group, GroupDto> groupConverter,
        IModelConverter<Channel, ChannelDto> channelConverter,
        ISpeakeasyV1HubService hubService,
        IModelConverter<CustomEmoji, CustomEmojiDto> emojiConverter) : 
        base(uow.GroupRepository, groupConverter, uow)
    {
        _channelConverter = channelConverter;
        _hubService = hubService;
        _emojiConverter = emojiConverter;
    }

    [HttpGet("{id}/channels")]
    public async Task<ActionResult<IEnumerable<ChannelDto>>> GetChannelsAsync(Guid id)
    {
        var group = await _repository.GetByIdAsync(id, trackEntity: false);
        if (group is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        var channels = await _unitOfWork.ChannelRepository.GetChannelsForGroup(id);

        return Ok(channels.Select(_channelConverter.ToTransmissionModel));
    }

    [HttpGet("{id}/emoji")]
    public async Task<ActionResult<IAsyncEnumerable<CustomEmojiDto>>> GetEmojisAsync(Guid id)
    {
        if (!await _unitOfWork.GroupRepository.ContainsAsync(id))
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        return Ok(_unitOfWork.CustomEmojiRepository.GetAllForGroupAsyncEnumerable(id)
            .Select(_emojiConverter.ToTransmissionModel));
    }

    [HttpGet]
    public ActionResult<IAsyncEnumerable<GroupDto>> GetAll()
    {
        return Ok(_unitOfWork.GroupRepository.GetAll().Select(_converter.ToTransmissionModel));
    }

    protected override async Task OnEntityCreatedAsync(GroupDto dto)
    {
        await _hubService.SendNotificationToAllAsync(e => e.GroupCreatedAsync(dto.Id!.Value));
    }

    protected override async Task OnEntityUpdatedAsync(Guid id)
    {
        await _hubService.SendNotificationToAllAsync(e => e.GroupUpdatedAsync(id));
    }

    protected override async Task OnEntityDeletedAsync(Guid id)
    {
        await _hubService.SendNotificationToAllAsync(e => e.GroupDeletedAsync(id));
    }
}