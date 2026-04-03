using Speakeasy.Server.Storage.Abstractions;
using Speakeasy.Server.Storage.Enums;

namespace Speakeasy.Server.Storage.Validators;

public class FileValidatorResult<T> : IFileValidatorResult<T>
{
    public required bool IsValid { get; set; }
    
    public required bool HasErrors { get; set; }
    
    public required bool HasWarnings { get; set; }
    
    public required IEnumerable<FileValidationError> Errors { get; set; }
    
    public required IEnumerable<string> Warnings { get; set; }
    
    public required T? FileProperties { get; set; }

    public static FileValidatorResult<T> FromErrors(params FileValidationError[] errors)
    {
        return new FileValidatorResult<T>()
        {
            Errors = errors,
            FileProperties = default,
            HasErrors = true,
            HasWarnings = false,
            IsValid = false,
            Warnings = [],
        };
    }
}