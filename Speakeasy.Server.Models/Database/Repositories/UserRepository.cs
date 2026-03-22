using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(SpeakeasyDbContext context) : 
        base(context.Users)
    {
    }
}