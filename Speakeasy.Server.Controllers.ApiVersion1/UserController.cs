using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;
using Speakeasy.Server.Storage.Abstractions;
using Speakeasy.Server.Storage.Enums;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class UserController : BaseV1ApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly IImageValidator _imageValidator;
    private readonly ITemporaryFileStore _temporaryFileStore;
    private readonly ICurrentUserProvider _currentUserProvider;
    
    public UserController(
        IUnitOfWork unitOfWork, 
        UserManager<User> userManager, 
        IImageValidator imageValidator,
        ITemporaryFileStore temporaryFileStore,
        ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _imageValidator = imageValidator;
        _temporaryFileStore = temporaryFileStore;
        _currentUserProvider = currentUserProvider;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(string id)
    {
        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id, trackEntities: false);
        if(user is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }
        
        return Ok(ToTransmissionModel(user));
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMeAsync()
    {
        var user = await _currentUserProvider.GetCurrentUserAsync();

        return Ok(ToTransmissionModel(user));
    }

    [HttpPost("profile-image")]
    [RequestSizeLimit(ControllerConstants.FileSizes.TwoMegaBytes)]
    public async Task<ActionResult> UploadProfileImageAsync([FromForm] IFormFile file)
    {
        if (file.Length is 0)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedFileLengthNotValid));
        }

        var user = await _currentUserProvider.GetCurrentUserAsync();

        await using var temporaryFile = _temporaryFileStore.CreateTemporaryFile();
        var temporaryFileStream = temporaryFile.GetStream();

        await file.CopyToAsync(temporaryFileStream);

        temporaryFileStream.Position = 0;
        var validatorResult =  await _imageValidator.ValidateAsync(temporaryFileStream);

        if (!validatorResult.IsValid)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedImageNotValid));
        }

        if (validatorResult.HasErrors || validatorResult.HasWarnings)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedImageNotValid));
        }

        if (validatorResult.FileProperties!.PixelHeight > ControllerConstants.Limits.ProfilePicture.Height ||
            validatorResult.FileProperties.PixelWidth > ControllerConstants.Limits.ProfilePicture.Width)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedImageTooBig));
        }
        
        if (user.ProfilePicture is not null)
        {
            await _unitOfWork.FileRepository.DeleteFileById(user.ProfilePicture.Id);
        }

        var storedFile = new StoredFile()
        {
            Id = Guid.NewGuid(),
            MimeType = validatorResult.FileProperties!.MimeType,
            FileCategory = FileCategory.Image,
            FileExtension = validatorResult.FileProperties.ImageType.ToString().ToLower(),
            OriginalFileName = file.FileName,
        };

        temporaryFileStream.Position = 0;
        await _unitOfWork.FileRepository.AddFileAsync(temporaryFileStream, storedFile);
        await _unitOfWork.CommitAsync();
        
        user.ProfilePicture = storedFile;
        await _userManager.UpdateAsync(user);

        return Ok();
    }

    private UserDto ToTransmissionModel(User user)
    {
        return new UserDto()
        {
            Id = user.Id,
            DisplayName = user.UserName,
            ProfileImage = user.ProfilePicture?.Id,
        };
    }
    
}