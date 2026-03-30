using Microsoft.AspNetCore.SignalR;
using Speakeasy.Server.Models.Transmission;

namespace Speakeasy.Server.Controllers.ApiVersion1.Hubs;

public interface ISpeakeasyV1Client
{
    [HubMethodName("MessageReceived")]
    Task MessageReceivedAsync(Guid channelId, MessageDto message);
   
    [HubMethodName("ChannelCreated")] 
    Task ChannelCreatedAsync(Guid channelId);

    [HubMethodName("ChannelDeleted")]
    Task ChannelDeletedAsync(Guid channelId);

    [HubMethodName("ChannelUpdated")]
    Task ChannelUpdatedAsync(Guid channelId);

    [HubMethodName("GroupCreated")]
    Task GroupCreatedAsync(Guid groupId);

    [HubMethodName("GroupDeleted")]
    Task GroupDeletedAsync(Guid groupId);

    [HubMethodName("GroupUpdated")]
    Task GroupUpdatedAsync(Guid groupId);

}