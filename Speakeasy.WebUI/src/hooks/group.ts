import { useNavigate, useParams } from "@solidjs/router";

import { useAppContext } from "@context/appContext";
import { getCurrentGroupId, navigateToGroup } from "@utilities/route";
import { type GroupDto, postApiV1Channel, postApiV1Group } from "@api";

export function useGroupState() {
  const context = useAppContext();
  const params = useParams();
  const navigate = useNavigate();

  const selectedGroupId = getCurrentGroupId(params);

  return {
    channels: context.channels()[selectedGroupId || ""] || [],
    createChannel: async (newChannelname: string) => {
      if (!selectedGroupId) {
        return;
      }

      const channelResponse = await postApiV1Channel({
        body: {
          groupId: selectedGroupId,
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

      await context.loadChannels(selectedGroupId);
      navigateToGroup(navigate, selectedGroupId, channelResponse.data.id!);
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
    selectedGroup: selectedGroupId
      ? context.groups[selectedGroupId]
      : undefined,
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
