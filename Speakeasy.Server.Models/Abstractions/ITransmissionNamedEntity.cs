namespace Speakeasy.Server.Models.Abstractions;

public interface ITransmissionNamedEntity : ITransmissionEntity
{
    public string Name { get; set; }
}