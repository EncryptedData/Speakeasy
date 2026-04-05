import { NavigateOptions, Params } from "@solidjs/router";

export function getCurrentGroupId(params: Params) {
  return params.groupId;
}

export function getCurrentChannelId(params: Params): string | undefined {
  return params.channelId;
}

export function navigateToGroup(
  navigate: (to: string | number, options?: Partial<NavigateOptions>) => void,
  groupId: string,
  channelId?: string,
) {
  let url = `/${groupId}`;

  if (channelId) {
    url += `/${channelId}`;
  }

  navigate(url);
}
