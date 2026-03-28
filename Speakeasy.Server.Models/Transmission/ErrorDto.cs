using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Speakeasy.Server.Models.Transmission;

public class ErrorDto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ErrorCode Error { get; set; }

    public static ErrorDto FromCode(ErrorCode errorCode)
    {
        return new ErrorDto()
        {
            Error = errorCode,
        };
    }
}