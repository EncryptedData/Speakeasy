import { Component, Show } from "solid-js";

import { useUserState } from "@hooks/user";
import { DefaultProfilePicture } from "./defaultProfilePicture";

export type ChatProfileProps = {
  userId: string;
};

export const ChatProfile: Component<ChatProfileProps> = (props) => {
  const [user, loading] = useUserState(props.userId);
  return (
    <Show when={!loading()} fallback={<div />}>
      {user().profileImage ? (
        <div class="profile rounded-full">{user().profileImage}</div>
      ) : (
        <DefaultProfilePicture
          class="profile"
          displayName={user().displayName || user().id!}
        />
      )}
    </Show>
  );
};
