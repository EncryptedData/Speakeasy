using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class ChannelController : BaseRepositoryController<Channel, ChannelDto>
{
    private readonly IModelConverter<Message, MessageDto> _messageConverter;
    private readonly UserManager<User> _userManager;
    
    public ChannelController(IUnitOfWork uow,
        IModelConverter<Channel, ChannelDto> channelConverter,
        IModelConverter<Message, MessageDto> messageConverter,
        UserManager<User> userManager) : 
        base(uow.ChannelRepository, channelConverter, uow)
    {
        _messageConverter = messageConverter;
        _userManager = userManager;
    }

    public override async Task<ActionResult> PutAsync(ChannelDto dto)
    {
        if (!await _unitOfWork.ChannelRepository.ContainsAsync(dto.GroupId))
        {
            return BadRequest(new ErrorDto()
            {
                Error = ErrorCode.EntityNotFound,
            });
        }

        return await base.PutAsync(dto);
    }

    public override async Task<ActionResult<ChannelDto>> PostAsync(ChannelDto dto)
    {
        if (!await _unitOfWork.GroupRepository.ContainsAsync(dto.GroupId))
        {
            return BadRequest(new ErrorDto()
            {
                Error = ErrorCode.EntityNotFound,
            });
        }
        
        return await base.PostAsync(dto);
    }

    [HttpGet("{id:guid}/messages")]
    public async Task<ActionResult<IAsyncEnumerable<MessageDto>>> GetMessagesAsync(
        Guid id,
        [FromQuery] GetMessagesQueryParameters parameters)
    {
        var channel = await _unitOfWork.ChannelRepository.GetByIdAsync(id, trackEntity: false);
        if (channel is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        if (parameters.LastMessageId is not null)
        {
            var message = await _unitOfWork.MessageRepository.GetByIdAsync(parameters.LastMessageId.Value, trackEntity: false);
            if (message is null)
            {
                return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
            }
        }

        var messages = await _unitOfWork.MessageRepository.GetMessagesForChannelAsync(id,
            parameters.LastMessageId, parameters.Take ?? ModelConstants.Limits.DefaultMessageFetchLimit);

        var messageDtos = messages.Select(_messageConverter.ToTransmissionModel);
        return Ok(messageDtos);
    }

    [HttpPost("{id:guid}/message")]
    public async Task<ActionResult> PostNewMessageAsync(Guid id,
        [FromBody] [MaxLength(ModelConstants.Limits.MaxMessageLength)] string text)
    {
        var channel = await _repository.GetByIdAsync(id);
        if (channel is null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        ArgumentNullException.ThrowIfNull(user);

        var newMessage = new Message()
        {
            Id = Guid.NewGuid(),
            Author = user,
            Channel = channel,
            CreatedOn = DateTime.UtcNow,
            CurrentText = text,
            Edits = [],
            HasBeenEdited = false,
            IsDeleted = false,
            LastEditedOn = null,
        };

        await _unitOfWork.MessageRepository.AddAsync(newMessage);
        await _unitOfWork.CommitAsync();

        return NoContent();
    }
}