import { Component } from "solid-js";

import { User } from "@models/User";

export type ChatProfileProps = {
  user: User;
};

export const ChatProfile: Component<ChatProfileProps> = (props) => {
  return <div>{props.user?.username}</div>;
};
