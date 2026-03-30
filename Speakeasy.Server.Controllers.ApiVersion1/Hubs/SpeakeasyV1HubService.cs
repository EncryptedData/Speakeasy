using Microsoft.AspNetCore.SignalR;

namespace Speakeasy.Server.Controllers.ApiVersion1.Hubs;

public class SpeakeasyV1HubService : ISpeakeasyV1HubService
{
    private readonly IHubContext<SpeakeasyV1Hub, ISpeakeasyV1Client> _hubContext;

    public SpeakeasyV1HubService(IHubContext<SpeakeasyV1Hub, ISpeakeasyV1Client> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task SendNotificationToGroupAsync(string group, Func<ISpeakeasyV1Client, Task> message)
    {
        await message(_hubContext.Clients.Group(group));
    }

    public async Task SendNotificationToAllAsync(Func<ISpeakeasyV1Client, Task> message)
    {
        await message(_hubContext.Clients.All);
    }
}