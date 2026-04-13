using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface ICurrentUserProvider
{
    Task<User> GetCurrentUserAsync();
}