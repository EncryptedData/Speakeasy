using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Common.Exceptions;
using Speakeasy.Server.Controllers.ApiVersion1.Hubs;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;
using Speakeasy.Server.Models.Transmission.Post;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class GroupController : BaseRepositoryController<Group, GroupDto>
{
    private readonly IModelConverter<Channel, ChannelDto> _channelConverter;
    private readonly IModelConverter<GroupRole, GroupRoleDto> _roleConverter;
    private readonly IModelConverter<CustomEmoji, CustomEmojiDto> _emojiConverter;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ISpeakeasyV1HubService _hubService;
    
    public GroupController(
        IUnitOfWork uow, 
        IModelConverter<Group, GroupDto> groupConverter,
        IModelConverter<Channel, ChannelDto> channelConverter,
        ISpeakeasyV1HubService hubService, 
        IModelConverter<GroupRole, GroupRoleDto> roleConverter,
        IModelConverter<CustomEmoji, CustomEmojiDto> emojiConverter,
        ICurrentUserProvider currentUserProvider) : 
        base(uow.GroupRepository, groupConverter, uow)
    {
        _channelConverter = channelConverter;
        _hubService = hubService;
        _emojiConverter = emojiConverter;
        _currentUserProvider = currentUserProvider;
        _roleConverter = roleConverter;
    }

    [HttpGet("{id}/roles")]
    public async Task<ActionResult<IEnumerable<GroupRoleDto>>> GetRoles(Guid id)
    {
        var group = await _unitOfWork.GroupRepository.GetByIdAsync(id);

        if (group is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        return Ok(group.Roles.Select(_roleConverter.ToTransmissionModel));
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

    [HttpPost("{groupId:guid}/user/add")]
    public async Task<ActionResult> AddUserToGroup(Guid groupId, [FromBody] GroupClaimPostDto dto)
    {
        if (!Enum.TryParse(dto.Permission, out GroupUserClaimPermission parsedPermission))
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.InvalidParameter));
        }
        
        var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
        if (group is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }
        
        var currentUser = await _currentUserProvider.GetCurrentUserAsync();
        ExceptionUtil.ThrowIfNull<Exception>(currentUser);
        
        var currentUserClaim = group.Claims.FirstOrDefault(e => e.User.Id == currentUser.Id);
        if (currentUserClaim is null || currentUserClaim.Permission != GroupUserClaimPermission.Admin)
        {
            return Forbid();
        }
        
        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(dto.UserId);
        
        if (user is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }
        
        var existingClaim = group.Claims.FirstOrDefault(e => e.User.Id == user.Id);

        if (existingClaim is not null)
        {
            if (existingClaim.Permission == parsedPermission)
            {
                return BadRequest(ErrorDto.FromCode(ErrorCode.ClaimAlreadyExists));
            }

            if (existingClaim.Permission == GroupUserClaimPermission.Admin)
            {
                if (group.Claims.Count(e => e.Permission == GroupUserClaimPermission.Admin) == 1)
                {
                    return Conflict(ErrorDto.FromCode(ErrorCode.SoleAdminCannotBeRemoved));
                }
            }

            existingClaim.Permission = parsedPermission;
        }
        else
        {
            var claim = new GroupUserClaim
            {
                Id = Guid.NewGuid(),
                Group = group,
                Permission = parsedPermission,
                User = user,
            };
        
            group.Claims.Add(claim);
            await _unitOfWork.GroupRepository.AddClaimAsync(claim);
        }

        await _unitOfWork.CommitAsync();
        
        return NoContent();
    }
    
    [HttpPost("{groupId:guid}/user/remove")]
    public async Task<ActionResult> RemoveUserFromGroup(Guid groupId, [FromBody] GroupClaimPostDto dto)
    {
        var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
        if (group is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }
        
        var currentUser = await _currentUserProvider.GetCurrentUserAsync();
        ExceptionUtil.ThrowIfNull<Exception>(currentUser);
        
        var currentUserClaim = group.Claims.FirstOrDefault(e => e.User.Id == currentUser.Id);

        if (currentUserClaim is null)
        {
            return Forbid();
        }
        
        if(currentUserClaim.Permission != GroupUserClaimPermission.Admin && currentUserClaim.User.Id != dto.UserId)
        {
            return Forbid();
        }
        
        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(dto.UserId);
        
        if (user is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }
        
        var existingClaim = group.Claims.FirstOrDefault(e => e.User.Id == user.Id);
        
        if(existingClaim is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        if (existingClaim.Permission == GroupUserClaimPermission.Admin)
        {
            if (group.Claims.Count(e => e.Permission == GroupUserClaimPermission.Admin) == 1)
            {
                return Conflict(ErrorDto.FromCode(ErrorCode.SoleAdminCannotBeRemoved));
            }
        }
        
        _unitOfWork.GroupRepository.RemoveClaim(existingClaim);
        group.Claims.Remove(existingClaim);
        await _unitOfWork.CommitAsync();
        
        return NoContent();
    }

    public override async Task<ActionResult> DeleteAsync(Guid id)
    {
        var group = await _unitOfWork.GroupRepository.GetByIdAsync(id);

        if (group is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        foreach (var groupClaim in group.Claims)
        {
            _unitOfWork.GroupRepository.RemoveClaim(groupClaim);
        }
        
        _repository.Remove(group);
        
        await _unitOfWork.CommitAsync();
        await OnEntityDeletedAsync(id);
        return NoContent();
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