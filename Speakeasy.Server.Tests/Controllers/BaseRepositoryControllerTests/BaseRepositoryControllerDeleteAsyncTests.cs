using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Tests.Controllers.BaseRepositoryControllerTests;

public class BaseRepositoryControllerDeleteAsyncTests : BaseRepositoryControllerTestBase
{
    [Test]
    public async Task ReturnsNotFoundWhenEntityNotInRepository()
    {
        SetEntityInRepository(null);

        var result = await Unit.DeleteAsync(ExampleEntity.Id);

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeOfType<ErrorDto>()
            .Which.Error.Should().Be(ErrorCode.EntityNotFound);
    }

    [Test]
    public async Task CallsRepositoryRemoveOnEntity()
    {
        await Unit.DeleteAsync(ExampleEntity.Id);
        
        _mockTestRepository.Verify(e => e.Remove(ExampleEntity), Times.Once);
    }

    [Test]
    public async Task CallsUnitOfWorkCommit()
    {
        await Unit.DeleteAsync(ExampleEntity.Id);
        
        _mockUnitOfWork.Verify(e => e.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ReturnsNoContentResultOnSuccess()
    {
        var result = await Unit.DeleteAsync(ExampleEntity.Id);

        result.Should().BeOfType<NoContentResult>();
    }
}