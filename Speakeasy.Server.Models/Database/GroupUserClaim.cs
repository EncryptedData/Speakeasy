using Microsoft.EntityFrameworkCore;

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
public class GroupUserClaim
{
    public Guid Id { get; set; }
    public Group Group { get; set; }
    public User User { get; set; }
    public GroupUserClaimPermission Permission { get; set; }
    
}