import { NavigateOptions, Params } from "@solidjs/router";

export function getCurrentGroupId(params: Params) {
  return params.groupId;
}

export function navigateToGroup(
  navigate: (to: string | number, options?: Partial<NavigateOptions>) => void,
  groupId: string,
) {
  navigate(`/${groupId}`);
}
