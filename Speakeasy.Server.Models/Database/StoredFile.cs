using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Abstractions;
#pragma warning disable CS8618

namespace Speakeasy.Server.Models.Database;

[PrimaryKey(nameof(Id))]
public class StoredFile : IEntity
{
    public Guid Id { get; set; }
    
    public string OriginalFileName { get; set; }
    
    public string FileExtension { get; set; }
    
    public string MimeType { get; set; }

    public FileCategory FileCategory { get; set; }
}