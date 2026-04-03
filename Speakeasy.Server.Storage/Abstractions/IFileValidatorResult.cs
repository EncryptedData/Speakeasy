using Speakeasy.Server.Storage.Enums;

namespace Speakeasy.Server.Storage.Abstractions;

public interface IFileValidatorResult<T>
{
    bool IsValid { get; set; }
    
    bool HasErrors { get; set; }
    
    bool HasWarnings { get; set; }
    
    IEnumerable<FileValidationError> Errors { get; set; }
    
    IEnumerable<string> Warnings { get; set; }

    T? FileProperties { get; set; }
}