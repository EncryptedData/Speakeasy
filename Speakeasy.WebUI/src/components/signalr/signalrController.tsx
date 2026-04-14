import { batch, createEffect, onCleanup, VoidComponent } from "solid-js";

import { useAuthContext } from "@context/authContext";
import { useAppContext } from "@context/appContext";
import { useChatContext } from "@context/chatContext";
import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import {
  getApiV1ChannelById,
  getApiV1GroupById,
  MessageDto,
} from "@api";
import { produce } from "solid-js/store";
import { parseDateFromUuid } from "@utilities/uuid";

export function createSignalRClient() {
  const c = new HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_API_URL}/hub/v1`)
    .build();

  if (import.meta.env.DEV) {
    c.onclose((error) => console.error("SignalR disconnected", error));
    c.onreconnected(() => console.info("SignalR reconnected"));
    c.on("send", (data) => console.info("SignalR sent message", data));
  }

  return c;
}

const client = createSignalRClient();

export const SignalRController: VoidComponent = () => {
  const authContext = useAuthContext();
  const appContext = useAppContext();
  const chatContext = useChatContext();

  function getChannelPosition(channelId: string):
    | {
        groupId: string;
        channelIndex: number;
      }
    | undefined {
    for (const groupId of Object.keys(appContext.groups)) {
      const channels = appContext.channels()[groupId];
      const foundIndex = channels?.findIndex((c) => c.id === channelId);
      if (foundIndex !== undefined && foundIndex > -1) {
        return {
          groupId,
          channelIndex: foundIndex,
        };
      }
    }

    return undefined;
  }

  function onMessageReceived(channelId: string, message: MessageDto) {
    console.log(channelId, message);

    if (message.authorId === authContext.me()?.id) {
      return;
    }

    const messages = chatContext.messages()[channelId];
    if (!messages) {
      return;
    }

    chatContext.setShift(true);
    chatContext.updateMessages(
      produce((m) => {
        const innerMessages = m[channelId]!;

        for (let i = innerMessages.length - 1; i > 0; i--) {
          const currentMessage = innerMessages[i]!;

          const createdOn = parseDateFromUuid(message.id!);
          if (currentMessage.createdOn < createdOn) {
            innerMessages.splice(i + 1, 0, {
              author: message.authorId!,
              createdOn: createdOn,
              currentText: message.currentText,
              id: message.id!,
            });
            return;
          }
        }
      }),
    );
  }

  async function onChannelCreated(channelId: string) {
    const response = await getApiV1ChannelById({
      path: {
        id: channelId,
      },
    });

    if (response.error) {
      console.error(
        "signalr: failed to GET channel",
        channelId,
        response.error,
      );
      return;
    } else if (response.data.createdBy === authContext.me().id) {
      // Logic which creates the channel handles reloading state
      return;
    }

    appContext.updateChannels(
      produce((c) => {
        const groupValue = c[response.data.groupId];
        if (groupValue) {
          groupValue.push(response.data);
        } else {
          c[response.data.groupId] = [response.data];
        }
      }),
    );
  }

  function onChannelDeleted(channelId: string) {
    const channelPosition = getChannelPosition(channelId);
    if (!channelPosition) {
      return;
    }

    appContext.updateChannels(
      produce((c) =>
        c[channelPosition.groupId]?.splice(channelPosition.channelIndex, 1),
      ),
    );
  }

  async function onChannelUpdated(channelId: string) {
    const response = await getApiV1ChannelById({
      path: {
        id: channelId,
      },
    });

    if (response.error) {
      console.error(
        "signalr: failed to GET channel",
        channelId,
        response.error,
      );
      return;
    }

    const storedChannels = appContext.channels()[response.data.groupId];
    if (!storedChannels) {
      // We haven't loaded the channels yet; when we do we'll get the new value
      return;
    }

    const matchingChannelIndex = storedChannels.findIndex(
      (c) => c.id === channelId,
    );

    if (matchingChannelIndex > -1) {
      appContext.updateChannels(
        response.data.groupId,
        matchingChannelIndex,
        response.data,
      );
    } else {
      appContext.loadChannels(response.data.groupId);
    }
  }

  function onGroupCreated(_groupId: string) {
    appContext.loadGroups();
  }

  function onGroupDeleted(groupId: string) {
    batch(() => {
      appContext.updateGroups(produce((g) => delete g[groupId]));
      appContext.updateChannels(produce((g) => delete g[groupId]));
    });
  }

  async function onGroupUpdated(groupId: string) {
    const response = await getApiV1GroupById({
      path: {
        id: groupId,
      },
    });

    if (response.error) {
      console.error("signalr: failed to GET group", groupId, response.error);
      return;
    }

    appContext.updateGroups(groupId, response.data);
  }

  const handlers: [string, (...args: any[]) => void][] = [
    ["MessageReceived", onMessageReceived],
    ["ChannelCreated", onChannelCreated],
    ["ChannelDeleted", onChannelDeleted],
    ["ChannelUpdated", onChannelUpdated],
    ["GroupCreated", onGroupCreated],
    ["GroupDeleted", onGroupDeleted],
    ["GroupUpdated", onGroupUpdated],
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
