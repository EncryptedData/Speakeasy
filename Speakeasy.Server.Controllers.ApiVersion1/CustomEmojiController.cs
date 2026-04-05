using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;
using Speakeasy.Server.Models.Transmission.Post;
using Speakeasy.Server.Storage.Abstractions;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class CustomEmojiController : BaseRepositoryController<CustomEmoji, CustomEmojiDto>
{
    private IImageValidator _imageValidator;
    private ITemporaryFileStore _temporaryFileStore;
    private UserManager<User> _userManager;
    
    public CustomEmojiController(
        IModelConverter<CustomEmoji, CustomEmojiDto> converter, 
        IUnitOfWork uow,
        IImageValidator imageValidator,
        ITemporaryFileStore temporaryFileStore,
        UserManager<User> userManager) : 
        base(uow.CustomEmojiRepository, converter, uow)
    {
        _imageValidator = imageValidator;
        _temporaryFileStore = temporaryFileStore;
        _userManager = userManager;
    }

    
    public override Task<ActionResult<CustomEmojiDto>> PostAsync(CustomEmojiDto dto)
    {
        // The PostFormAsync() overrides this using Order property
        throw new NotSupportedException();
    }

    [HttpPost(Order = -1)]
    [RequestSizeLimit(ControllerConstants.FileSizes.MegaByte)]
    public async Task<ActionResult<CustomEmojiDto>> PostFormAsync([FromForm] CustomEmojiUploadFormDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        ArgumentNullException.ThrowIfNull(user);
        
        if (dto.File.Length is 0)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedFileLengthNotValid));
        }
        
        // Check if the group exists
        Group? group = null;
        if (dto.GroupId is not null)
        {
            group = await _unitOfWork.GroupRepository.GetByIdAsync(dto.GroupId.Value);

            if (group is null)
            {
                return BadRequest(ErrorDto.FromCode(ErrorCode.EntityNotFound));
            }
        }
        
        // Now check that the name hasn't been used somewhere else
        if (await _unitOfWork.CustomEmojiRepository.ContainsNameAsync(dto.Name))
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.NameAlreadyTaken));
        }

        await using var temporaryFile = _temporaryFileStore.CreateTemporaryFile();
        var temporaryFileStream = temporaryFile.GetStream();
        await dto.File.CopyToAsync(temporaryFileStream);
        temporaryFileStream.Position = 0;

        var validatorResult = await _imageValidator.ValidateAsync(temporaryFileStream);

        if (!validatorResult.IsValid || validatorResult.HasErrors)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedImageNotValid));
        }

        if (validatorResult.FileProperties!.PixelHeight > ControllerConstants.Limits.Emoji.Height ||
            validatorResult.FileProperties.PixelWidth > ControllerConstants.Limits.Emoji.Width)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedImageTooBig));
        }
        
        StoredFile storedFile = new()
        {
            Id = Guid.NewGuid(),
            FileCategory = FileCategory.Image,
            FileExtension = validatorResult.FileProperties.ImageType.ToString(),
            MimeType = validatorResult.FileProperties.MimeType,
            OriginalFileName = dto.File.FileName,
        };

        temporaryFileStream.Position = 0;
        await _unitOfWork.FileRepository.AddFileAsync(temporaryFileStream, storedFile);

        CustomEmoji customEmoji = new()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            CreatedOn = DateTime.UtcNow,
            Image = storedFile,
            Group = group,
            Author = user,
        };

        await _unitOfWork.CustomEmojiRepository.AddAsync(customEmoji);
        await _unitOfWork.CommitAsync();

        var dtoModel = _converter.ToTransmissionModel(customEmoji);

        return Ok(dtoModel);
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