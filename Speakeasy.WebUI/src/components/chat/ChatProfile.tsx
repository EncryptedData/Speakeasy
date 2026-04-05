import { Component, Show } from "solid-js";

import { useUserState } from "@hooks/user";
import { useProfileImage } from "@hooks/profileImage";
import { DefaultProfilePicture } from "./defaultProfilePicture";

export type ChatProfileProps = {
  userId: string;
};

export const ChatProfile: Component<ChatProfileProps> = (props) => {
  const [user, loading] = useUserState(props.userId);
  const imageUrl = useProfileImage(() => user().profileImage);

  return (
    <Show when={!loading()} fallback={<div />}>
      {imageUrl() ? (
        <img class="profile rounded-full" src={imageUrl()} />
      ) : (
        <DefaultProfilePicture
          class="profile"
          displayName={user().displayName || user().id!}
        />
      )}
    </Show>
  );
};
