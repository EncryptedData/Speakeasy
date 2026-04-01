using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class DownloadController : BaseV1ApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public DownloadController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FileStreamResult>> GetAsync(Guid id)
    {
        var file = await _unitOfWork.FileRepository.GetFileByIdAsync(id);
        if (file is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        var stream = await _unitOfWork.FileRepository.GetFileStreamByIdAsync(id);
        ArgumentNullException.ThrowIfNull(stream);

        return File(stream, file.MimeType);
    }
}