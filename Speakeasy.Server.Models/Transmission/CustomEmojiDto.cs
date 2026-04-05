using System.ComponentModel.DataAnnotations;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Transmission;

public class CustomEmojiDto : ITransmissionNamedEntity
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(ModelConstants.Limits.MaxNameLength)]
    public string Name { get; set; }
    
    public Guid? GroupId { get; set; }
    
    public Guid? Image { get; set; }
    
    public string? AuthorId { get; set; }
    
    public DateTime? CreatedOn { get; set; }
}