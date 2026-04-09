using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Controllers.ApiVersion1.Hubs;
using Speakeasy.Server.Models;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;
using Speakeasy.Server.Models.Transmission.Post;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class MessageController : BaseRepositoryController<Message, MessageDto>
{
    private readonly ISpeakeasyV1HubService _hubService;
    private readonly UserManager<User> _userManager;

    public MessageController(
        IUnitOfWork uow,
        IModelConverter<Message, MessageDto> converter,
        ISpeakeasyV1HubService hubService,
        UserManager<User> userManager) : 
        base(uow.MessageRepository, converter, uow)
    {
        _hubService = hubService;
        _userManager = userManager;
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

    [HttpPost("{id}/react")]
    public async Task<ActionResult<MessageReactionDto>> ReactMessageAsync(Guid id, [FromBody] MessageReactionPostDto dto)
    {
        if (dto.CustomEmojiId is null && dto.Emoji is null)
        {
            ModelState.AddModelError(nameof(dto.CustomEmojiId), "Either CustomEmojiId or Emoji must be set");
            ModelState.AddModelError(nameof(dto.Emoji), "Either Emoji or CustomEmojiId must be set");
        }

        if (dto.CustomEmojiId is not null && dto.Emoji is not null)
        {
            ModelState.AddModelError(string.Empty, "Only one field can be specified. Either CustomEmojiId or Emoji.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.GetUserAsync(User);
        ArgumentNullException.ThrowIfNull(user);
        
        CustomEmoji? customEmoji = null;
        if (dto.CustomEmojiId is not null)
        {
            customEmoji = await _unitOfWork.CustomEmojiRepository.GetByIdAsync(dto.CustomEmojiId.Value);

            if (customEmoji is null)
            {
                return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
            }
        }
        
        var message = await _repository.GetByIdAsync(id);
        if (message is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        var reaction = dto.CustomEmojiId is not null
            ? message.Reactions.FirstOrDefault(e => e.CustomEmoji?.Id == customEmoji?.Id)
            : message.Reactions.FirstOrDefault(e => e.EmojiChar == dto.Emoji);

        if (reaction is null)
        {
            if (message.Reactions.Count + 1 > ModelConstants.Limits.EmojiCharacterStringLength)
            {
                return Conflict(ErrorDto.FromCode(ErrorCode.TooManyMessageReactions));
            }

            reaction = new MessageReaction()
            {
                Id = Guid.CreateVersion7(),
                CustomEmoji = customEmoji,
                EmojiChar = dto.Emoji,
                Message = message,
                Reactors = [],
            };
            
            message.Reactions.Add(reaction);
            await _unitOfWork.MessageRepository.AddMessageReactionAsync(reaction);
        }
        // Quit out if they already reacted to this message
        else if (reaction.Reactors.Any(e => e.Id == user.Id))
        {
            return Ok(ToMessageReactionDto(reaction));
        }
        
        reaction.Reactors.Add(user);
        await _unitOfWork.CommitAsync();

        return Ok(ToMessageReactionDto(reaction));
    }

    [HttpPost("{id}/unreact")]
    public async Task<ActionResult> UnreactMessageAsync(Guid id, [FromBody] MessageReactionPostDto dto)
    {
        if (dto.CustomEmojiId is null && dto.Emoji is null)
        {
            ModelState.AddModelError(nameof(dto.CustomEmojiId), "Either CustomEmojiId or Emoji must be set");
            ModelState.AddModelError(nameof(dto.Emoji), "Either Emoji or CustomEmojiId must be set");
        }

        if (dto.CustomEmojiId is not null && dto.Emoji is not null)
        {
            ModelState.AddModelError(string.Empty, "Only one field can be specified. Either CustomEmojiId or Emoji.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var user = await _userManager.GetUserAsync(User);
        ArgumentNullException.ThrowIfNull(user);

        var message = await _repository.GetByIdAsync(id);
        if (message is null)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        var reaction =
            message.Reactions.FirstOrDefault(e => e.CustomEmoji?.Id == dto.CustomEmojiId || e.EmojiChar == dto.Emoji);
        if (reaction is null)
        {
            return NoContent();
        }

        reaction.Reactors.RemoveAll(e => e.Id == user.Id);

        if (reaction.Reactors.Count < 1)
        {
            message.Reactions.RemoveAll(e => e.Id == reaction.Id);
            _unitOfWork.MessageRepository.RemoveMessageReaction(reaction);
        }

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

    private MessageReactionDto ToMessageReactionDto(MessageReaction reaction)
    {
        return new MessageReactionDto()
        {
            CustomEmojiId = reaction.CustomEmoji?.Id,
            Emoji = reaction.EmojiChar,
            Users = reaction.Reactors.Select(e => e.Id).ToList(),
        };
    }
}