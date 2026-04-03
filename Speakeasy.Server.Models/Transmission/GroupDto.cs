using System.ComponentModel.DataAnnotations;
using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Transmission;

public class GroupDto : ITransmissionNamedEntity
{
    public Guid? Id { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    [Length(1, ModelConstants.Limits.MaxNameLength)]
    public string Name { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    
    public List<Guid>? Channels { get; set; }
}