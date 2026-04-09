using System.ComponentModel.DataAnnotations;

namespace Speakeasy.Server.Models.Transmission.Post;

public class MessageReactionPostDto
{
    public Guid? CustomEmojiId { get; set; }
    
    [MaxLength(ModelConstants.Limits.EmojiCharacterStringLength)]
    [RegularExpression("^" + GEmojiSharp.Emoji.RegexPattern + "$")]
    public string? Emoji { get; set; }
}