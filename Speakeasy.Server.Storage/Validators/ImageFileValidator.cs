using System.Net.Mime;
using SixLabors.ImageSharp;
using Speakeasy.Server.Storage.Abstractions;
using Speakeasy.Server.Storage.Enums;

namespace Speakeasy.Server.Storage.Validators;

public class ImageFileValidator : IImageValidator
{
    public async Task<IFileValidatorResult<ImageValidationProperties>> ValidateAsync(Stream stream,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var imageFormat = await Image.DetectFormatAsync(stream, cancellationToken);

            ImageType imageType = imageFormat.DefaultMimeType switch
            {
                MediaTypeNames.Image.Gif => ImageType.Gif,
                MediaTypeNames.Image.Jpeg => ImageType.Jpeg,
                MediaTypeNames.Image.Webp => ImageType.Webp,
                MediaTypeNames.Image.Png => ImageType.Png,
                _ => ImageType.Unknown,
            };

            return new FileValidatorResult<ImageValidationProperties>()
            {
                Errors = [],
                FileProperties = new ImageValidationProperties()
                {
                    MimeType = imageFormat.DefaultMimeType,
                    ImageType = imageType,
                },
                HasErrors = false,
                HasWarnings = false,
                IsValid = true,
                Warnings = []
            };
        }
        catch (Exception ex) when // NotSupportedException included: "The stream is not readable or the image format is not supported."
            (ex is InvalidImageContentException or UnknownImageFormatException or NotSupportedException)
        {
            return FileValidatorResult<ImageValidationProperties>.FromErrors(FileValidationError.InvalidFormat);
        }
    }
}