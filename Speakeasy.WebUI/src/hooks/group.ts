import { useNavigate, useParams } from "@solidjs/router";

import { useAppContext } from "@context/appContext";
import { getCurrentGroupId, navigateToGroup } from "@utilities/route";
import { type GroupDto, postApiV1Group } from "@api";

export function useGroupState() {
  const context = useAppContext();
  const params = useParams();
  const navigate = useNavigate();

  const selectedGroupId = getCurrentGroupId(params);

  return {
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
