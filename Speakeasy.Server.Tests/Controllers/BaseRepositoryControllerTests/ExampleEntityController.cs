using Speakeasy.Server.Controllers.ApiVersion1;

namespace Speakeasy.Server.Tests.Controllers.BaseRepositoryControllerTests;

public class ExampleEntityController : BaseRepositoryController<ExampleEntity, ExampleEntityDto>
{
    public ExampleEntityController(
        IRepository<ExampleEntity> repository,
        IModelConverter<ExampleEntity, ExampleEntityDto> converter, 
        IUnitOfWork uow) : 
        base(repository, converter, uow)
    {
    }

    protected override Task OnEntityCreatedAsync(ExampleEntityDto dto)
    {
        return Task.CompletedTask;
    }

    protected override Task OnEntityUpdatedAsync(Guid id)
    {
        return Task.CompletedTask;
    }

    protected override Task OnEntityDeletedAsync(Guid id)
    {
        return Task.CompletedTask;
    }
}