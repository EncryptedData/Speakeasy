using System.ComponentModel.DataAnnotations;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Transmission;

public class MessageReactionDto
{
    public Guid? CustomEmojiId { get; set; }
    
    public string? Emoji { get; set; }
    
    /// <summary>
    /// Users who used this reaction on this message
    /// </summary>
    public required IEnumerable<string> Users { get; set; }
}