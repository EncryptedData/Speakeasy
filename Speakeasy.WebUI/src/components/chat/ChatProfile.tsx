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
        <img class="profile rounded-full" src={user().profileImage || ""} />
      ) : (
        <DefaultProfilePicture
          class="profile"
          displayName={user().displayName || user().id!}
        />
      )}
    </Show>
  );
};
