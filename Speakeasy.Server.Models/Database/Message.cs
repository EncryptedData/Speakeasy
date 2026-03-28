using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
public class Message : IEntity
{
    public Guid Id { get; set; }
    
    public User Author { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? LastEditedOn { get; set; }
    
    public bool HasBeenEdited { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public string CurrentText { get; set; }
    
    public List<MessageEdits> Edits { get; set; }
    
    public Channel Channel { get; set; }
}