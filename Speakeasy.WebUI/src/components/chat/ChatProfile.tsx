import { Component, Show } from "solid-js";

import { useUserState } from "@hooks/user";
import { useProfileImage } from "@hooks/profileImage";
import { DefaultProfilePicture } from "./defaultProfilePicture";
import { clsx } from "@utilities/class";

export type ChatProfileProps = {
  size?: "sm" | "md" | "lg";
  userId: string;
};

export const ChatProfile: Component<ChatProfileProps> = (props) => {
  const [user, loading] = useUserState(props.userId);
  const imageUrl = useProfileImage(() => user().profileImage);

  return (
    <Show when={!loading()} fallback={<div />}>
      {imageUrl() ? (
        <img
          class={clsx("profile rounded-full", props.size)}
          src={imageUrl()}
        />
      ) : (
        <DefaultProfilePicture
          class={clsx("profile", props.size)}
          displayName={user().displayName || user().id!}
        />
      )}
    </Show>
  );
};
