namespace Speakeasy.Server.Models.Abstractions;

public interface IModelConverter<TDatabase, TTransmission>
    where TDatabase : class, IEntity
    where TTransmission : class, ITransmissionEntity
{
    Task<TDatabase> ToDatabaseModelAsync(IUnitOfWork uow, TTransmission dto);
    
    TTransmission ToTransmissionModel(TDatabase entity);
    
    void UpdateDatabaseModelAsync(TDatabase entity, TTransmission dto);
}