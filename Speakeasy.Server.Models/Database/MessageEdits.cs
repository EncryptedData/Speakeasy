using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
public class MessageEdits : IEntity
{
    public Guid Id { get; set; }
    
    public Message Message { get; set; }
    
    public DateTime EditedOn { get; set; }
    
    public string Text { get; set; }
}