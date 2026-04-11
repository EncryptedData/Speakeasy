namespace Speakeasy.Server.Tests.Controllers.BaseRepositoryControllerTests;

[TestFixture]
[UnitTestCategory]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class BaseRepositoryControllerTestBase
{
    protected Mock<IRepository<ExampleEntity>> _mockTestRepository = new();
    protected Mock<IUnitOfWork> _mockUnitOfWork = new();
    protected Mock<IModelConverter<ExampleEntity, ExampleEntityDto>> _mockModelConverter = new();

    protected ExampleEntity ExampleEntity { get; private set; } = new()
    {
        Id = Guid.Parse("70BA5B4C-C140-4B1C-9192-D385CF88F388"),
        PropertyHiddenInDto = 42,
    };

    protected ExampleEntityDto ExampleEntityDto { get; private set; } = new()
    {
        Id = Guid.Parse("70BA5B4C-C140-4B1C-9192-D385CF88F388"),
    };
    
    protected ExampleEntityController Unit { get; private set; }
    
    [SetUp]
    public void OnSetup()
    {
        SetEntityInRepository(ExampleEntity);
        SetModelConverterResponse(ExampleEntityDto);

        _mockModelConverter.Setup(e => e.ToDatabaseModelAsync(It.IsAny<IUnitOfWork>(), It.IsAny<ExampleEntityDto>()))
            .ReturnsAsync(() => ExampleEntity);
        
        Unit = new(_mockTestRepository.Object, _mockModelConverter.Object, _mockUnitOfWork.Object);
    }

    protected void SetEntityInRepository(ExampleEntity? testEntity)
    {
        _mockTestRepository.Setup(e => e.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(() => testEntity);
    }

    protected void SetModelConverterResponse(ExampleEntityDto? dto)
    {
        _mockModelConverter.Setup(e => e.ToTransmissionModel(It.IsAny<ExampleEntity>())).Returns(() => dto);
    }

    protected void PreparePostAsyncTest()
    {
        ExampleEntityDto.Id = null;
    }
}