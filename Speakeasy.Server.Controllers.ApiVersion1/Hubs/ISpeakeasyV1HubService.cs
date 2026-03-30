namespace Speakeasy.Server.Controllers.ApiVersion1.Hubs;

public interface ISpeakeasyV1HubService
{
    Task SendNotificationToGroupAsync(string group, Func<ISpeakeasyV1Client, Task> message);

    Task SendNotificationToAllAsync(Func<ISpeakeasyV1Client, Task> message);
}