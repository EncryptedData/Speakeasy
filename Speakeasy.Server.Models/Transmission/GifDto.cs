using Speakeasy.Server.Models.Abstractions;

namespace Speakeasy.Server.Models.Transmission;

public class GifDto : ITransmissionNamedEntity
{
    public Guid? Id { get; set; }
    
    public string Name { get; set; }
    
    public Guid? GroupId { get; set; }
    
    public List<string>? Tags { get; set; }
    
    public Guid? ImageId { get; set; }
    
    public string? AuthorId { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime UpdatedOn { get; set; }
}