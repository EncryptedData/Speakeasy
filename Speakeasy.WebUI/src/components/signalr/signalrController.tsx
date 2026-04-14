import { HubConnectionState } from "@microsoft/signalr";
import { createEffect, onCleanup, VoidComponent } from "solid-js";

import { useAppContext } from "@context/appContext";
import { useAuthContext } from "@context/authContext";
import { useChatContext } from "@context/chatContext";
import {
  createChannelCreatedHandler,
  createChannelDeletedHandler,
  createChannelUpdatedHandler,
  createGroupCreatedHandler,
  createGroupDeletedHandler,
  createGroupUpdatedHandler,
  createMessageReceivedHandler,
  createSignalRClient,
} from "./signalrHandlers";

const client = createSignalRClient();

export const SignalRController: VoidComponent = () => {
  const authContext = useAuthContext();
  const appContext = useAppContext();
  const chatContext = useChatContext();

  const handlers: [string, (...args: any[]) => void][] = [
    [
      "MessageReceived",
      createMessageReceivedHandler(
        () => authContext.me()?.id,
        chatContext.messages,
        chatContext.setShift,
        chatContext.updateMessages,
      ),
    ],
    [
      "ChannelCreated",
      createChannelCreatedHandler(
        () => authContext.me()?.id,
        appContext.updateChannels,
      ),
    ],
    [
      "ChannelDeleted",
      createChannelDeletedHandler(
        () => appContext.groups,
        appContext.channels,
        appContext.updateChannels,
      ),
    ],
    [
      "ChannelUpdated",
      createChannelUpdatedHandler(
        appContext.channels,
        appContext.updateChannels,
        appContext.loadChannels,
      ),
    ],
    ["GroupCreated", createGroupCreatedHandler(appContext.loadGroups)],
    [
      "GroupDeleted",
      createGroupDeletedHandler(
        appContext.updateGroups,
        appContext.updateChannels,
      ),
    ],
    ["GroupUpdated", createGroupUpdatedHandler(appContext.updateGroups)],
  ];

  function onClientConnected() {
    handlers.forEach(([event, handler]) => client.on(event, handler));
  }

  function cleanup() {
    handlers.forEach(([event, handler]) => client.off(event, handler));
  }

  createEffect(async () => {
    if (!authContext.isLoggedIn()) {
      if (client.state !== HubConnectionState.Disconnected) {
        await client.stop();
      }
      cleanup();
    } else {
      try {
        await client.start();
        onClientConnected();
      } catch (error) {
        console.error("SignalR failed to connect", error);
      }
    }
  });

  onCleanup(() => {
    client.stop();
    cleanup();
  });

  return null;
};
