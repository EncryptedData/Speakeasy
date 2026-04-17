using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database;

public class User : IdentityUser
{
    public StoredFile? ProfilePicture { get; set; }
    
    public List<GroupUserClaim> GroupClaims { get; set; }
}