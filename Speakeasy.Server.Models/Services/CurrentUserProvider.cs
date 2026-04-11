using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Speakeasy.Server.Common.Exceptions;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Exceptions;

namespace Speakeasy.Server.Models.Services;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public CurrentUserProvider(
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager,
        IUnitOfWork unitOfWork)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<User> GetCurrentUserAsync()
    {
        ExceptionUtil.ThrowIfNull<Exception>(_httpContextAccessor.HttpContext);
        
        var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
        ExceptionUtil.ThrowIfNull(id, () => new UserNotFoundException());

        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
        ExceptionUtil.ThrowIfNull(user, () => new UserNotFoundException(id));

        return user;
    }
}