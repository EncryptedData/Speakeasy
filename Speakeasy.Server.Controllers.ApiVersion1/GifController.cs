using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Common.Exceptions;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;
using Speakeasy.Server.Models.Transmission.Post;
using Speakeasy.Server.Storage.Abstractions;
using Speakeasy.Server.Storage.Enums;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class GifController : BaseRepositoryController<Gif, GifDto>
{
    private readonly IImageValidator _imageValidator;
    private readonly ITemporaryFileStore _temporaryFileStore;
    private readonly ICurrentUserProvider _currentUserProvider;
    
    public GifController(
        IModelConverter<Gif, GifDto> converter, 
        IUnitOfWork uow, 
        IImageValidator imageValidator, 
        ITemporaryFileStore temporaryFileStore,
        ICurrentUserProvider currentUserProvider) : 
        base(uow.GifRepository, converter, uow)
    {
        _imageValidator = imageValidator;
        _temporaryFileStore = temporaryFileStore;
        _currentUserProvider = currentUserProvider;
    }

    public override Task<ActionResult<GifDto>> PostAsync(GifDto dto)
    {
        // Overridden via PostGifAsync action
        throw new NotSupportedException();
    }

    [HttpPost(Order = -1)]
    [RequestSizeLimit(ControllerConstants.FileSizes.SixTeenMegabytes)]
    public async Task<ActionResult<GifDto>> PostGifAsync([FromForm] GifUploadFormPostDto form)
    {
        if (form.File.Length < 1)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedFileLengthNotValid));
        }

        Group? group = null;
        if (form.GroupId is not null)
        {
            group = await _unitOfWork.GroupRepository.GetByIdAsync(form.GroupId.Value);

            if (group is null)
            {
                return BadRequest(ErrorDto.FromCode(ErrorCode.EntityNotFound));
            }
        }

        await using var temporaryFile = _temporaryFileStore.CreateTemporaryFile();
        await using var temporaryFileStream = temporaryFile.GetStream();

        await form.File.CopyToAsync(temporaryFileStream);
        temporaryFileStream.Position = 0;

        var validatorResult = await _imageValidator.ValidateAsync(temporaryFileStream);
        temporaryFileStream.Position = 0;

        if (!validatorResult.IsValid || validatorResult.HasErrors)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedImageNotValid));
        }
        
        ExceptionUtil.ThrowIfNull<Exception>(validatorResult.FileProperties);

        if (validatorResult.FileProperties.ImageType is not (ImageType.Gif or ImageType.Webp))
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedImageNotValid));
        }

        if (validatorResult.FileProperties.PixelHeight > ControllerConstants.Limits.Gif.Height ||
            validatorResult.FileProperties.PixelWidth > ControllerConstants.Limits.Gif.Width)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.UploadedImageTooBig));
        }

        StoredFile storedFile = new()
        {
            Id = Guid.NewGuid(),
            FileCategory = FileCategory.Image,
            FileExtension = validatorResult.FileProperties.ImageType.ToString(),
            MimeType = validatorResult.FileProperties.MimeType,
            OriginalFileName = form.File.FileName,
        };
        await _unitOfWork.FileRepository.AddFileAsync(temporaryFileStream, storedFile);

        var now = DateTime.UtcNow;
        var user = await _currentUserProvider.GetCurrentUserAsync();
        
        Gif gif = new()
        {
            Tags = form.Tags?.Select(e => e.ToLower()).ToList() ?? [],
            Group = group,
            Id = Guid.NewGuid(),
            Name = form.Name,
            StoredFile = storedFile,
            UpdatedOn = now,
            CreatedOn = now,
            Author = user,
        };
        
        await _unitOfWork.GifRepository.AddAsync(gif);
        await _unitOfWork.CommitAsync();

        var dto = _converter.ToTransmissionModel(gif);
        return Ok(dto);
    }

    public override async Task<ActionResult> DeleteAsync(Guid id)
    {
        Gif? gif = await _repository.GetByIdAsync(id);

        if (gif is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        await _unitOfWork.FileRepository.DeleteFileById(gif.StoredFile.Id);
        _unitOfWork.GifRepository.Remove(gif);
        await _unitOfWork.CommitAsync();

        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IAsyncEnumerable<GifDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IAsyncEnumerable<GifDto>>> GetAsync(
        Guid? groupId,
        [FromQuery(Name = "tag")] string[]? tags,
        int skip = 0,
        [Range(1, ControllerConstants.Limits.Gif.MaximumTake)]int take = ControllerConstants.Limits.Gif.MaximumTake)
    {
        Group? group = null;
        if (groupId is not null)
        {
            group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId.Value);
        }

        if (tags is not null && !tags.Any())
        {
            tags = null;
        }

        var enumerable = _unitOfWork.GifRepository
            .GetAsyncEnumerable(skip, take, group, tags)
            .Select(_converter.ToTransmissionModel);

        return Ok(enumerable);
    }

    protected override Task OnEntityCreatedAsync(GifDto dto)
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