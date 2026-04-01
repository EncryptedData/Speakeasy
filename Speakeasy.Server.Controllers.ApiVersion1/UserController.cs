using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public class UserController : BaseV1ApiController
{
    private readonly IUnitOfWork _unitOfWork;
    
    public UserController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(string id)
    {
        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id, trackEntities: false);
        if(user is null)
        {
            return NotFound();
        }
        
        return Ok(ToTransmissionModel(user));
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