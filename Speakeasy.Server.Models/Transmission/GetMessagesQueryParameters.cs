using System.ComponentModel.DataAnnotations;

namespace Speakeasy.Server.Models.Transmission;

public class GetMessagesQueryParameters
{
    [Range(1, ModelConstants.Limits.MaxMessageFetchLimit)]
    public int? Take { get; set; }
    
    public Guid? LastMessageId { get; set; }
}