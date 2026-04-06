import { useNavigate, useParams } from "@solidjs/router";
import { createMemo } from "solid-js";

import { useAppContext } from "@context/appContext";
import {
  createGroupUrl,
  getCurrentChannelId,
  getCurrentGroupId,
  navigateToGroup,
} from "@utilities/route";
import { type GroupDto, postApiV1Channel, postApiV1Group } from "@api";

export function useGroupState() {
  const context = useAppContext();
  const params = useParams();
  const navigate = useNavigate();

  const selectedGroupId = createMemo(() => getCurrentGroupId(params));
  const selectedChannelId = createMemo(() => getCurrentChannelId(params));

  return {
    channels: () => context.channels()[selectedGroupId() || ""] || [],
    createChannel: async (newChannelname: string) => {
      const selectedGroup = selectedGroupId();
      if (!selectedGroup) {
        return;
      }

      const channelResponse = await postApiV1Channel({
        body: {
          groupId: selectedGroup,
          name: newChannelname,
        },
      });

      if (channelResponse.error) {
        console.error("failed to create channel", channelResponse.error);
        return undefined;
      } else if (!channelResponse.data) {
        console.error("failed to create channel; no response");
        return undefined;
      }

      await context.loadChannels(selectedGroup);
      navigateToGroup(navigate, selectedGroup, channelResponse.data.id!);
    },
    createGroup: async (newGroupName: string) => {
      const response = await createGroup(newGroupName);

      if (!response?.id) {
        //;
        return;
      }

      await context.loadGroups();
      navigateToGroup(navigate, response.id);
    },
    getGroupUrl: (groupId: string, channelId?: string) => {
      if (!groupId) {
        return "/";
      }

      if (channelId) {
        return createGroupUrl(groupId, channelId);
      } else {
        const channels = context.channels()[groupId];

        return createGroupUrl(groupId, channels?.[0]?.id);
      }
    },
    selectGroup: (groupId: string) => {
      const group = context.groups[groupId];
      if (!group) {
        return;
      }

      const groupChannels = context.channels()[selectedGroupId() || ""] || [];
      const firstChannelId = groupChannels[0]?.id;

      navigateToGroup(navigate, group.id!, firstChannelId);
    },
    selectedChannel: () =>
      selectedGroupId() && selectedChannelId()
        ? context
            .channels()
            [selectedGroupId()!]?.find((c) => c.id === selectedChannelId())
        : undefined,
    selectedGroup: () =>
      selectedGroupId ? context.groups[selectedGroupId()!] : undefined,
  };
}

export async function createGroup(
  groupName: string,
): Promise<GroupDto | undefined> {
  const response = await postApiV1Group({
    body: {
      name: groupName,
    },
  });

  if (response.error) {
    console.error("failed to create group", response.error);
    return undefined;
  } else if (!response.data) {
    console.error("failed to create group; no response");
    return undefined;
  }

  return response.data;
}
