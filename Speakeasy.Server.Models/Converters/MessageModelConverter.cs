using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Models.Converters;

public class MessageModelConverter : IModelConverter<Message, MessageDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MessageModelConverter(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<Message> ToDatabaseModelAsync(IUnitOfWork uow, MessageDto dto)
    {
        ArgumentNullException.ThrowIfNull(_httpContextAccessor.HttpContext);

        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        ArgumentNullException.ThrowIfNull(user);

        return new Message()
        {
            CreatedOn = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            Author = user,
            CurrentText = dto.CurrentText,
            Edits = [],
            HasBeenEdited = false,
            IsDeleted = false,
            LastEditedOn = null,
        };
    }

    public MessageDto ToTransmissionModel(Message entity)
    {
        if (entity.IsDeleted)
        {
            return new MessageDto()
            {
                CreatedOn = entity.CreatedOn,
                IsDeleted = entity.IsDeleted,
            };
        }
        
        return new MessageDto()
        {
            CreatedOn = entity.CreatedOn,
            Id = entity.Id,
            CurrentText = entity.CurrentText,
            HasBeenEdited = entity.HasBeenEdited,
            IsDeleted = entity.IsDeleted,
            LastEditedOn = entity.LastEditedOn,
            AuthorId = entity.Author.Id,
        };
    }

    public void UpdateDatabaseModelAsync(Message entity, MessageDto dto)
    {
        if (!entity.CurrentText.Equals(dto.CurrentText, StringComparison.InvariantCulture))
        {
            var now = DateTime.UtcNow;
            
            entity.Edits.Add(new MessageEdits()
            {
                Id = Guid.NewGuid(),
                EditedOn = now,
                Message = entity,
                Text = entity.CurrentText,
            });

            entity.CurrentText = dto.CurrentText;
            entity.HasBeenEdited = true;
            entity.LastEditedOn = now;
        }
    }
}