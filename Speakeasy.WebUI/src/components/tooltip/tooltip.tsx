import { JSX, ParentComponent } from "solid-js";
import CorvuTooltip, { RootProps } from "@corvu/tooltip";

export type ToolTipProps = {
  content: JSX.Element;
  openDelay?: number;
  placement: RootProps["placement"];
};

export const Tooltip: ParentComponent<ToolTipProps> = (props) => {
  return (
    <CorvuTooltip
      placement={props.placement}
      openDelay={props.openDelay ?? 200}
      floatingOptions={{
        offset: 4,
        flip: true,
        shift: true,
      }}
    >
      <CorvuTooltip.Trigger>{props.children}</CorvuTooltip.Trigger>
      <CorvuTooltip.Portal>
        <CorvuTooltip.Content class="p-2 bg-bg-elevated rounded-md">
          {props.content} <CorvuTooltip.Arrow class="text-bg-elevated" />
        </CorvuTooltip.Content>
      </CorvuTooltip.Portal>
    </CorvuTooltip>
  );
};
