using Speakeasy.Server.Models.Abstractions;
#pragma warning disable CS8618

namespace Speakeasy.Server.Models.Database;

public class Gif : INamedEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Group? Group { get; set; }
    
    public List<string> Tags { get; set; }
    
    public StoredFile StoredFile { get; set; }
    
    public User Author { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime UpdatedOn { get; set; }
}