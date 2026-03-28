using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Models.Converters;

public class GroupModelConverter : IModelConverter<Group, GroupDto>
{
    public Task<Group> ToDatabaseModelAsync(IUnitOfWork uow, GroupDto dto)
    {
        return Task.FromResult(new Group()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Channels = [],
            CreatedOn = DateTime.UtcNow,
        });
    }

    public GroupDto ToTransmissionModel(Group entity)
    {
        return new GroupDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Channels = entity.Channels.Select(e => e.Id).ToList(),
            CreatedOn = entity.CreatedOn,
        };
    }

    public void UpdateDatabaseModelAsync(Group entity, GroupDto dto)
    {
        entity.Name = dto.Name;
    }
}