namespace Speakeasy.Server.Storage.Abstractions;

public interface IFileValidator<T>
{
    Task<IFileValidatorResult<T>> ValidateAsync(Stream stream, CancellationToken cancellationToken = default);
}