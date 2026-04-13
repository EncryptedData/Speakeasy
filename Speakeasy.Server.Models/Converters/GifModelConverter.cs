using Speakeasy.Server.Common.Exceptions;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Models.Converters;

public class GifModelConverter : IModelConverter<Gif, GifDto>
{
    public Task<Gif> ToDatabaseModelAsync(IUnitOfWork uow, GifDto dto)
    {
        // We will override the controller's POST functionality and handle it ourselves.
        throw new NotSupportedException();
    }

    public GifDto ToTransmissionModel(Gif entity)
    {
        return new GifDto()
        {
            AuthorId = entity.Author.Id,
            CreatedOn = entity.CreatedOn,
            GroupId = entity.Group?.Id,
            Id = entity.Id,
            ImageId = entity.StoredFile?.Id,
            Name = entity.Name,
            Tags = entity.Tags.ToList(),
            UpdatedOn = entity.UpdatedOn,
        };
    }

    public void UpdateDatabaseModelAsync(Gif entity, GifDto dto)
    {
        bool entityWasUpdated = false;

        if (!entity.Name.Equals(dto.Name, StringComparison.InvariantCulture))
        {
            entityWasUpdated = true;
            entity.Name = dto.Name;
        }

        if (dto.Tags is not null)
        {
            entityWasUpdated = true;
            entity.Tags = dto.Tags.Select(e => e.ToLower()).ToList();
        }

        if (entityWasUpdated)
        {
            entity.UpdatedOn = DateTime.UtcNow;
        }
    }
}