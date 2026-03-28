using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;
#pragma warning disable CS8618

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
[Index(nameof(Name))]
public class Group : INamedEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public List<Channel> Channels { get; set; }
}