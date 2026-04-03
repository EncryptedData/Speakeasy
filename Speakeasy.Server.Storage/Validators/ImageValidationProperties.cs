using Speakeasy.Server.Storage.Enums;

namespace Speakeasy.Server.Storage.Validators;

public class ImageValidationProperties
{
    public ImageType ImageType { get; set; }
    
    public string MimeType { get; set; }
    
    public int PixelWidth { get; set; }
    
    public int PixelHeight { get; set; }
}