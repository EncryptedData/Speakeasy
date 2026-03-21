namespace Speakeasy.Server.Models.Abstractions;

public interface INamedEntity : IEntity
{
    string Name { get; set; }
}