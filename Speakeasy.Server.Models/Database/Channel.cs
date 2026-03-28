using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;
#pragma warning disable CS8618

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
public class Channel : INamedEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public Group Group { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public User CreatedBy { get; set; }
    
    public List<Message> Messages { get; set; }
    
    
}