using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

#pragma warning disable CS8618

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
public class GroupRoleClaim : IEntity
{
    public Guid Id { get; set; }
    
    public GroupRole GroupRole { get; set; }
    
    public User User { get; set; }
}