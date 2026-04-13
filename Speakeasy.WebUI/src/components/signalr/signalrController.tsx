import { batch, createEffect, onCleanup, VoidComponent } from "solid-js";

import { useAuthContext } from "@context/authContext";
import { useAppContext } from "@context/appContext";
import { useChatContext } from "@context/chatContext";
import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import {
  getApiV1ChannelById,
  getApiV1Group,
  getApiV1GroupById,
  MessageDto,
} from "@api";
import { produce } from "solid-js/store";
import { parseDateFromUuid } from "@utilities/uuid";

const client = new HubConnectionBuilder()
  .withUrl(`${import.meta.env.VITE_API_URL}/hub/v1`)
  .build();

if (import.meta.env.DEV) {
  client.onclose((error) => {
    console.error(`SignalR disconnected`, error);
  });

  client.onreconnected(() => {
    console.info("SignalR reconnected");
  });

  client.on("send", (data) => {
    console.info("SignalR sent message", data);
  });
}

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
      if (!!foundIndex && foundIndex > -1) {
        return {
          groupId,
          channelIndex: foundIndex,
        };
      }
    }

    return undefined;
  }

  function onMessageReceived(channelId: string, message: MessageDto) {
    if (message.authorId === authContext.me()?.id) {
      return;
    }

    const messages = chatContext.messages()[channelId];
    if (!messages) {
      return;
    }

    chatContext.updateMessages(
      produce((m) => {
        const innerMessages = m[channelId]!;

        for (let i = innerMessages.length; i > 0; i--) {
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

    if (matchingChannelIndex) {
      appContext.updateChannels(
        response.data.groupId,
        matchingChannelIndex,
        response.data,
      );
    } else {
      appContext.loadChannels(response.data.groupId);
    }
  }

  function onGroupCreated(groupId: string) {
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

  function onClientConnected() {
    client.on("MessageReceived", onMessageReceived);
    client.on("ChannelCreated", onChannelCreated);
    client.on("ChannelDeleted", onChannelDeleted);
    client.on("ChannelUpdated", onChannelUpdated);
    client.on("GroupCreated", onGroupCreated);
    client.on("GroupDeleted", onGroupDeleted);
    client.on("GroupUpdated", onGroupUpdated);
  }
  function cleanup() {
    client.off("MessageReceived", onMessageReceived);
    client.off("ChannelCreated", onChannelCreated);
    client.off("ChannelDeleted", onChannelDeleted);
    client.off("ChannelUpdated", onChannelUpdated);
    client.off("GroupCreated", onGroupCreated);
    client.off("GroupDeleted", onGroupDeleted);
    client.off("GroupUpdated", onGroupUpdated);
  }

  createEffect(async () => {
    if (
      !authContext.isLoggedIn() &&
      client.state !== HubConnectionState.Disconnected
    ) {
      await client.stop();
      onClientConnected();
    } else {
      await client.start();
      cleanup();
    }
  });

  onCleanup(() => {
    client.stop();
    cleanup();
  });

  return null;
};
