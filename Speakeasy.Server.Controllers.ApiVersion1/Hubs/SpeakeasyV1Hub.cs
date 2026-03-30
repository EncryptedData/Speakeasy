using Microsoft.AspNetCore.SignalR;

namespace Speakeasy.Server.Controllers.ApiVersion1.Hubs;

public class SpeakeasyV1Hub : Hub<ISpeakeasyV1Client>, ISpeakeasyV1Server
{

}