using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Speakeasy.Server.Models.Transmission.Post;

public class GifUploadFormPostDto
{
    [Required]
    public IFormFile File { get; set; }
    
    public Guid? GroupId { get; set; }
    
    [Required]
    [MaxLength(ModelConstants.Limits.MaxNumberOfTags)]
    public List<string> Tags { get; set; }
    
    [Required]
    [MaxLength(ModelConstants.Limits.MaxNameLength)]
    public string Name { get; set; }
}