using System.ComponentModel.DataAnnotations;
using GEmojiSharp;
using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;
#pragma warning disable CS8618

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
public class MessageReaction : IEntity
{
    public Guid Id { get; set; }
    
    public Message Message { get; set; }
    
    public CustomEmoji? CustomEmoji { get; set; }
    
    [MaxLength(ModelConstants.Limits.EmojiCharacterStringLength)]
    public string? EmojiChar { get; set; }
    
    public List<User> Reactors { get; set; }
}