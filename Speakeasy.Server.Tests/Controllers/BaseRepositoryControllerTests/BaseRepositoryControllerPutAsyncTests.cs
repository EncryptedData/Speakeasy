using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Tests.Controllers.BaseRepositoryControllerTests;

public class BaseRepositoryControllerPutAsyncTests : BaseRepositoryControllerTestBase
{
    [Test]
    public async Task ReturnsBadRequestWhenDtoIdIsNull()
    {
        ExampleEntityDto.Id = null;

        var result = await Unit.PutAsync(ExampleEntityDto);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ErrorDto>()
            .Which.Error.Should().Be(ErrorCode.MissingParameterId);
    }

    [Test]
    public async Task ReturnsNotFoundWhenEntityIsNotInDatabase()
    {
        SetEntityInRepository(null);

        var result = await Unit.PutAsync(ExampleEntityDto);

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeOfType<ErrorDto>()
            .Which.Error.Should().Be(ErrorCode.EntityNotFound);
    }

    [Test]
    public async Task CallsModelConverter()
    {
        await Unit.PutAsync(ExampleEntityDto);

        _mockModelConverter.Verify(e => e.UpdateDatabaseModelAsync(ExampleEntity, ExampleEntityDto), Times.Once);
    }

    [Test]
    public async Task CallCommitOnUnitOfWork()
    {
        await Unit.PutAsync(ExampleEntityDto);

        _mockUnitOfWork.Verify(e => e.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ReturnsNoContentWhenSuccess()
    {
        var result = await Unit.PutAsync(ExampleEntityDto);

        result.Should().BeOfType<NoContentResult>();
    }
}