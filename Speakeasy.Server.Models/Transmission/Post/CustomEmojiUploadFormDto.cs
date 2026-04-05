using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Speakeasy.Server.Models.Transmission.Post;

public class CustomEmojiUploadFormDto
{
    [Required]
    [Length(2, ModelConstants.Limits.MaxNameLength)]
    public string Name { get; set; }
    
    [Required]
    public IFormFile File { get; set; }
    
    public Guid? GroupId { get; set; }
}