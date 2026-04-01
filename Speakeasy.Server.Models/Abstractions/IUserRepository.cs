using Speakeasy.Server.Models.Database;

namespace Speakeasy.Server.Models.Abstractions;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string id, bool trackEntities = true);

    Task<bool> ContainsIdAsync(string id);

    Task AddAsync(User user);

    void Remove(User user);
}