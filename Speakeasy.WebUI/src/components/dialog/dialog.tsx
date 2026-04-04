import { Content, Label, Overlay, Portal, Root } from "@corvu/dialog";
import { ParentComponent } from "solid-js";

export type ControlledDialog = {
  open: boolean;
  onOpenChange: (newValue: boolean) => void;
};

export type DialogProps = ControlledDialog & {
  label: string;
};

export const Dialog: ParentComponent<DialogProps> = (props) => {
  return (
    <Root open={props.open} onOpenChange={props.onOpenChange}>
      <Portal>
        <Overlay class="fixed inset-0 bg-black/25 data-open:animate-in data-open:fade-in-0% data-closed:animate-out data-closed:fade-out-0%" />
        <Content class="fixed rounded-md left-1/2 top-1/2 bg-bg-base p-6 -translate-x-1/2 -translate-y-1/2 data-open:animate-in data-open:fade-in-0% data-open:zoom-in-95% data-open:slide-in-from-top-10% data-closed:animate-out data-closed:fade-out-0% data-closed:zoom-out-95% data-closed:slide-out-to-top-10% flex flex-col gap-2">
          <Label class="text-lg font-bold">{props.label}</Label>
          {props.children}
        </Content>
      </Portal>
    </Root>
  );
};
