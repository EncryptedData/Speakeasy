using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Transmission;

public class MessageDto : ITransmissionEntity
{
    public Guid? Id { get; set; }
    
    [Required]
    [MaxLength(ModelConstants.Limits.MaxMessageLength)]
    public string CurrentText { get; set; }
    
    public string? AuthorId { get; set; }
    
    public DateTime? LastEditedOn { get; set; }
    
    public bool? HasBeenEdited { get; set; }
    
    public bool? IsDeleted { get; set; }
    
    [JsonIgnore]
    public Guid? ChannelId { get; set; }
    
    public IEnumerable<MessageReactionDto>? Reactions { get; set; }
}