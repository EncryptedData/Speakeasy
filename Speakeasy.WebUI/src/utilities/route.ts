import { NavigateOptions, Params } from "@solidjs/router";

export function getCurrentGroupId(params: Params) {
  return params.groupId;
}

export function getCurrentChannelId(params: Params) {
  return () => params.channelId || "";
}

export function createGroupUrl(
  groupId: string,
  channelId?: string | null,
): string {
  let url = `/${groupId}`;

  if (channelId) {
    url += `/${channelId}`;
  }

  return url;
}

export function navigateToGroup(
  navigate: (to: string | number, options?: Partial<NavigateOptions>) => void,
  groupId: string,
  channelId?: string | null,
) {
  navigate(createGroupUrl(groupId, channelId));
}
