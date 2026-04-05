using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

#pragma warning disable CS8618

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
public class CustomEmoji : IEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public User Author { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public Group? Group { get; set; }
    
    public StoredFile Image { get; set; }
}