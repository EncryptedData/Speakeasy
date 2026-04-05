import { Component } from "solid-js";

import { Dialog } from "@components/dialog/dialog";
import { CreateGroupForm } from "./createGroupForm";

export type CreateGroupModalProps = {
  open: boolean;
  onOpenChange: (newValue: boolean) => void;
};

export const CreateGroupDialog: Component<CreateGroupModalProps> = (props) => {
  return (
    <Dialog label={"Create a Group"} {...props}>
      <CreateGroupForm onSubmit={() => props.onOpenChange(false)} />
    </Dialog>
  );
};
