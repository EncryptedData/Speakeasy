using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1;

public abstract class BaseRepositoryController<TDatabase, TTransmission> :
    BaseV1ApiController
    where TDatabase : class, IEntity
    where TTransmission : class, ITransmissionEntity
{
    protected readonly IRepository<TDatabase> _repository;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IModelConverter<TDatabase, TTransmission> _converter;

    protected BaseRepositoryController(IRepository<TDatabase> repository,
        IModelConverter<TDatabase, TTransmission> converter,
        IUnitOfWork uow)
    {
        _repository = repository;
        _converter = converter;
        _unitOfWork = uow;
    }

    [HttpGet("{id:guid}")]
    public virtual async Task<ActionResult<TTransmission>> GetAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }

        var dto = _converter.ToTransmissionModel(entity);

        return Ok(dto);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<ActionResult> PutAsync(
        [FromBody] TTransmission dto)
    {
        if (dto.Id is null)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.MissingParameterId));
        }
        
        var entity = await _repository.GetByIdAsync(dto.Id.Value);

        if (entity is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }
        
        _converter.UpdateDatabaseModelAsync(entity, dto);

        await _unitOfWork.CommitAsync();

        await OnEntityUpdatedAsync(entity.Id);

        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public virtual async Task<ActionResult<TTransmission>> PostAsync([FromBody] TTransmission dto)
    {
        if (dto.Id is not null)
        {
            return BadRequest(ErrorDto.FromCode(ErrorCode.EmptyParameterExpectedId));
        }

        var entity = await _converter.ToDatabaseModelAsync(_unitOfWork, dto);

        await _repository.AddAsync(entity);

        await _unitOfWork.CommitAsync();

        var updatedModel = _converter.ToTransmissionModel(entity);

        await OnEntityCreatedAsync(updatedModel);

        return Ok(updatedModel);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<ActionResult> DeleteAsync(Guid id)
    {
        var model = await _repository.GetByIdAsync(id);

        if (model is null)
        {
            return NotFound(ErrorDto.FromCode(ErrorCode.EntityNotFound));
        }
        
        _repository.Remove(model);
        
        await _unitOfWork.CommitAsync();

        await OnEntityDeletedAsync(id);

        return NoContent();
    }

    protected abstract Task OnEntityCreatedAsync(TTransmission dto);

    protected abstract Task OnEntityUpdatedAsync(Guid id);
    
    protected abstract Task OnEntityDeletedAsync(Guid id);
}