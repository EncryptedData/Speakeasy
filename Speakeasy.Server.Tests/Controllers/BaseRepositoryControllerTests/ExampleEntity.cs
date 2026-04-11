namespace Speakeasy.Server.Tests.Controllers.BaseRepositoryControllerTests;

public class ExampleEntity : IEntity
{
    public Guid Id { get; set; }
    
    public int PropertyHiddenInDto { get; set; }
}