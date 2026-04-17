using System.ComponentModel.DataAnnotations;

namespace Speakeasy.Server.Models.Transmission.Post;

public class GroupClaimPostDto
{
    [Required]
    public string UserId { get; set; }
    public string? Permission { get; set; }
}