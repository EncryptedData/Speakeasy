using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Models.Converters;

public class CustomEmojiModelConverter : IModelConverter<CustomEmoji, CustomEmojiDto>
{
    public Task<CustomEmoji> ToDatabaseModelAsync(IUnitOfWork uow, CustomEmojiDto dto)
    {
        throw new NotSupportedException();
    }

    public CustomEmojiDto ToTransmissionModel(CustomEmoji entity)
    {
        return new CustomEmojiDto()
        {
            GroupId = entity.Group?.Id,
            Image = entity.Image.Id,
            CreatedOn = entity.CreatedOn,
            Id = entity.Id,
            Name = entity.Name,
            AuthorId = entity.Author.Id,
        };
    }

    public void UpdateDatabaseModelAsync(CustomEmoji entity, CustomEmojiDto dto)
    {
        entity.Name = dto.Name;
    }
}