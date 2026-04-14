import { HubConnectionBuilder } from "@microsoft/signalr";
import { batch, Setter } from "solid-js";
import { produce, SetStoreFunction } from "solid-js/store";

import { getApiV1ChannelById, getApiV1GroupById, MessageDto } from "@api";
import { ChannelDictionary, GroupDictionary } from "@context/appContext";
import type { ChatMessage } from "@models/ChatMessage";
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

export function getChannelPosition(
  channelId: string,
  groups: GroupDictionary,
  channels: ChannelDictionary,
): { groupId: string; channelIndex: number } | undefined {
  for (const groupId of Object.keys(groups)) {
    const foundIndex = channels[groupId]?.findIndex((c) => c.id === channelId);
    if (foundIndex !== undefined && foundIndex > -1) {
      return { groupId, channelIndex: foundIndex };
    }
  }
  return undefined;
}

export function createMessageReceivedHandler(
  myId: () => string | null | undefined,
  messages: () => Record<string, ChatMessage[]>,
  setShift: Setter<boolean>,
  updateMessages: SetStoreFunction<Record<string, ChatMessage[]>>,
) {
  return (channelId: string, message: MessageDto) => {
    console.log(channelId, message);

    if (message.authorId === myId()) return;
    if (!messages()[channelId]) return;

    setShift(true);
    updateMessages(
      produce((m) => {
        const innerMessages = m[channelId]!;

        for (let i = innerMessages.length - 1; i >= 0; i--) {
          const currentMessage = innerMessages[i]!;
          const createdOn = parseDateFromUuid(message.id!);

          if (currentMessage.createdOn < createdOn) {
            innerMessages.splice(i + 1, 0, {
              author: message.authorId!,
              createdOn,
              currentText: message.currentText,
              id: message.id!,
            });
            return;
          }
        }
      }),
    );
  };
}

export function createChannelCreatedHandler(
  myId: () => string | null | undefined,
  updateChannels: SetStoreFunction<ChannelDictionary>,
) {
  return async (channelId: string) => {
    const response = await getApiV1ChannelById({ path: { id: channelId } });

    if (response.error) {
      console.error(
        "signalr: failed to GET channel",
        channelId,
        response.error,
      );
      return;
    } else if (response.data.createdBy === myId()) {
      // Logic which creates the channel handles reloading state
      return;
    }

    updateChannels(
      produce((c) => {
        const groupValue = c[response.data.groupId];
        if (groupValue) {
          groupValue.push(response.data);
        } else {
          c[response.data.groupId] = [response.data];
        }
      }),
    );
  };
}

export function createChannelDeletedHandler(
  groups: () => GroupDictionary,
  channels: () => ChannelDictionary,
  updateChannels: SetStoreFunction<ChannelDictionary>,
) {
  return (channelId: string) => {
    const channelPosition = getChannelPosition(channelId, groups(), channels());
    if (!channelPosition) return;

    updateChannels(
      produce((c) =>
        c[channelPosition.groupId]?.splice(channelPosition.channelIndex, 1),
      ),
    );
  };
}

export function createChannelUpdatedHandler(
  channels: () => ChannelDictionary,
  updateChannels: SetStoreFunction<ChannelDictionary>,
  loadChannels: (groupId: string) => Promise<void>,
) {
  return async (channelId: string) => {
    const response = await getApiV1ChannelById({ path: { id: channelId } });

    if (response.error) {
      console.error(
        "signalr: failed to GET channel",
        channelId,
        response.error,
      );
      return;
    }

    const storedChannels = channels()[response.data.groupId];
    if (!storedChannels) {
      // We haven't loaded the channels yet; when we do we'll get the new value
      return;
    }

    const matchingChannelIndex = storedChannels.findIndex(
      (c) => c.id === channelId,
    );

    if (matchingChannelIndex > -1) {
      updateChannels(
        response.data.groupId,
        matchingChannelIndex,
        response.data,
      );
    } else {
      loadChannels(response.data.groupId);
    }
  };
}

export function createGroupCreatedHandler(loadGroups: () => Promise<void>) {
  return (_groupId: string) => {
    loadGroups();
  };
}

export function createGroupDeletedHandler(
  updateGroups: SetStoreFunction<GroupDictionary>,
  updateChannels: SetStoreFunction<ChannelDictionary>,
) {
  return (groupId: string) => {
    batch(() => {
      updateGroups(produce((g) => delete g[groupId]));
      updateChannels(produce((g) => delete g[groupId]));
    });
  };
}

export function createGroupUpdatedHandler(
  updateGroups: SetStoreFunction<GroupDictionary>,
) {
  return async (groupId: string) => {
    const response = await getApiV1GroupById({ path: { id: groupId } });

    if (response.error) {
      console.error("signalr: failed to GET group", groupId, response.error);
      return;
    }

    updateGroups(groupId, response.data);
  };
}
