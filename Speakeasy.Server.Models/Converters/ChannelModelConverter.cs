using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Models.Converters;

public class ChannelModelConverter : IModelConverter<Channel, ChannelDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChannelModelConverter(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<Channel> ToDatabaseModelAsync(IUnitOfWork uow, ChannelDto dto)
    {
        ArgumentNullException.ThrowIfNull(_httpContextAccessor.HttpContext);
        
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        ArgumentNullException.ThrowIfNull(user);

        var group = await uow.GroupRepository.GetByIdAsync(dto.GroupId);
        ArgumentNullException.ThrowIfNull(group);

        return new Channel()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = user,
            Group = group,
            Messages = [],
        };
    }

    public ChannelDto ToTransmissionModel(Channel entity)
    {
        return new ChannelDto()
        {
            Name = entity.Name,
            Id = entity.Id,
            CreatedBy = entity.CreatedBy.Id,
            GroupId = entity.Group.Id,
        };
    }

    public void UpdateDatabaseModelAsync(Channel entity, ChannelDto dto)
    {
        entity.Name = dto.Name;
    }
}