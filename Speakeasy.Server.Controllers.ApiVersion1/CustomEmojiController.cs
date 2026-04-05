using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class CustomEmojiController : BaseRepositoryController<CustomEmoji, CustomEmojiDto>
{
    public CustomEmojiController(
        IModelConverter<CustomEmoji, CustomEmojiDto> converter, 
        IUnitOfWork uow) : 
        base(uow.CustomEmojiRepository, converter, uow)
    {
    }

    protected override Task OnEntityCreatedAsync(CustomEmojiDto dto)
    {
        return Task.CompletedTask;
    }

    protected override Task OnEntityUpdatedAsync(Guid id)
    {
        return Task.CompletedTask;
    }

    protected override Task OnEntityDeletedAsync(Guid id)
    {
        return Task.CompletedTask;
    }
}