using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Tests.Controllers.BaseRepositoryControllerTests;

public class BaseRepositoryControllerGetAsyncTests : BaseRepositoryControllerTestBase
{
    [Test]
    public async Task ReturnsNotFoundWhenEntityNotInDatabase()
    {
        SetEntityInRepository(null);

        var result = await Unit.GetByIdAsync(ExampleEntity.Id);
        
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeOfType<ErrorDto>()
            .Which.Error.Should().Be(ErrorCode.EntityNotFound);
    }

    [Test]
    public async Task ReturnsDtoModelFromConverter()
    {
        var result = await Unit.GetByIdAsync(ExampleEntity.Id);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(ExampleEntityDto);

        _mockModelConverter.Verify(e => e.ToTransmissionModel(ExampleEntity), Times.Once);
    }
    
    
}