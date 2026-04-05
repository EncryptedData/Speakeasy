using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;
using Speakeasy.Server.Models.Transmission.Post;
using Speakeasy.Server.Storage.Abstractions;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class CustomEmojiController : BaseRepositoryController<CustomEmoji, CustomEmojiDto>
{
    private IImageValidator _imageValidator;
    
    public CustomEmojiController(
        IModelConverter<CustomEmoji, CustomEmojiDto> converter, 
        IUnitOfWork uow,
        IImageValidator imageValidator) : 
        base(uow.CustomEmojiRepository, converter, uow)
    {
        _imageValidator = imageValidator;
    }

    
    public override Task<ActionResult<CustomEmojiDto>> PostAsync(CustomEmojiDto dto)
    {
        return Task.FromResult<ActionResult<CustomEmojiDto>>(NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<CustomEmojiDto>> PostFormAsync([FromForm] CustomEmojiUploadFormDto dto)
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