using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Tests.Controllers.BaseRepositoryControllerTests;

public class BaseRepositoryControllerPostAsyncTests : BaseRepositoryControllerTestBase
{
    [Test]
    public async Task ExpectsNoIdParameterDefined()
    {
        var result = await Unit.PostAsync(ExampleEntityDto);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ErrorDto>()
            .Which.Error.Should().Be(ErrorCode.EmptyParameterExpectedId);
    }

    [Test]
    public async Task CallsModelConverterToDatabaseModelAsync()
    {
        PreparePostAsyncTest();

        await Unit.PostAsync(ExampleEntityDto);

        _mockModelConverter.Verify(e => e.ToDatabaseModelAsync(_mockUnitOfWork.Object, ExampleEntityDto), Times.Once);
    }

    [Test]
    public async Task AddsNewDatabaseModelToRepository()
    {
        PreparePostAsyncTest();

        await Unit.PostAsync(ExampleEntityDto);
        
        _mockTestRepository.Verify(e => e.AddAsync(ExampleEntity), Times.Once);
    }

    [Test]
    public async Task CommitsUnitOfWork()
    {
        PreparePostAsyncTest();

        await Unit.PostAsync(ExampleEntityDto);

        _mockUnitOfWork.Verify(e => e.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CallModelConverterToTransmissionModel()
    {
        PreparePostAsyncTest();

        await Unit.PostAsync(ExampleEntityDto);

        _mockModelConverter.Verify(e => e.ToTransmissionModel(ExampleEntity), Times.Once);
    }

    [Test]
    public async Task ReturnsOkObjectWithEntityDto()
    {
        PreparePostAsyncTest();

        var result = await Unit.PostAsync(ExampleEntityDto);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(ExampleEntityDto);
    }
}