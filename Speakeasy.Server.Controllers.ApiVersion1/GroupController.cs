using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class GroupController : BaseRepositoryController<Group, GroupDto>
{
    private readonly IModelConverter<Channel, ChannelDto> _channelConverter;
    
    public GroupController(
        IUnitOfWork uow, 
        IModelConverter<Group, GroupDto> groupConverter,
        IModelConverter<Channel, ChannelDto> channelConverter) : 
        base(uow.GroupRepository, groupConverter, uow)
    {
        _channelConverter = channelConverter;
    }

    [HttpGet("{id}/channels")]
    public async Task<ActionResult> GetChannelsAsync(Guid id)
    {
        var group = await _repository.GetByIdAsync(id, trackEntity: false);
        if (group is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        var channels = await _unitOfWork.ChannelRepository.GetChannelsForGroup(id);

        return Ok(channels.Select(_channelConverter.ToTransmissionModel));
    }

    [HttpGet]
    public ActionResult<IAsyncEnumerable<GroupDto>> GetAll()
    {
        return Ok(_unitOfWork.GroupRepository.GetAll().Select(_converter.ToTransmissionModel));
    }
}