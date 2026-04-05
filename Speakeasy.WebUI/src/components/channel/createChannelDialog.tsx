import { Component } from "solid-js";

import { Dialog } from "@components/dialog/dialog";
import { CreateChannelForm } from "./createChannelForm";

export type CreateChannelModalProps = {
  open: boolean;
  onOpenChange: (newValue: boolean) => void;
};

export const CreateChannelDialog: Component<CreateChannelModalProps> = (
  props,
) => {
  return (
    <Dialog label={"Create a Channel"} {...props}>
      <CreateChannelForm onSubmit={() => props.onOpenChange(false)} />
    </Dialog>
  );
};
