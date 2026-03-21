using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
[Index(nameof(Name))]
public class User : INamedEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public bool IsDisabled { get; set; }
    
    public bool IsBot { get; set; }
    
    public string Password { get; set; }
}