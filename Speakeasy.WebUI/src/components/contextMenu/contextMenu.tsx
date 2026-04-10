import { createEffect, createSignal, For, ParentComponent } from "solid-js";
import Popover from "@corvu/popover";
import { MenuButton } from "@components/input/menuButton";

export type ContextMenuOption = {
  label: string;
  onClick: () => void;
  // TODO: Nested options -  options?: ContextMenuOption[];
};

export type ContextMenuProps = {
  options: ContextMenuOption[];
};

export const ContextMenu: ParentComponent<ContextMenuProps> = (props) => {
  const [isOpen, setIsOpen] = createSignal(false);
  const [anchorHandle, setAnchorHandle] = createSignal<
    HTMLDivElement | undefined
  >();

  createEffect(() => {
    const handle = anchorHandle();
    if (!handle) {
      return;
    }

    handle.oncontextmenu = (e) => {
      if (e.getModifierState("shift") || !props.options.length) {
        return;
      }

      e.preventDefault();
      setIsOpen(true);
    };
  });

  return (
    <Popover onOpenChange={setIsOpen} open={isOpen()} placement="right">
      <Popover.Anchor ref={setAnchorHandle}>{props.children}</Popover.Anchor>
      <Popover.Portal>
        <Popover.Overlay class="fixed inset-0 z-5" />
        <Popover.Content class="fixed z-6 rounded-lg bg-corvu-100 px-3 py-2 flex flex-col gap-2 bg-bg-elevated">
          <Popover.Arrow class="text-bg-elevated" />
          <For each={props.options}>
            {(item) => {
              return (
                <MenuButton
                  onClick={() => {
                    item.onClick();
                    setIsOpen(false);
                  }}
                >
                  {item.label}
                </MenuButton>
              );
            }}
          </For>
        </Popover.Content>
      </Popover.Portal>
    </Popover>
  );
};
