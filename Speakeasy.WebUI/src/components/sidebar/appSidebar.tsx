import "./appSidebar.css";

import { createEffect, createSignal, For } from "solid-js";

import { CreateChannelDialog } from "@components/channel/createChannelDialog";
import { CreateGroupDialog } from "@components/group/createGroupDialog";
import { Button } from "@components/input/button";
import { useAuthContext } from "@context/authContext";
import { useGroupState } from "@hooks/group";
import { useAppContext } from "@context/appContext";
import { DefaultProfilePicture } from "@components/chat/defaultProfilePicture";
import { clsx } from "@utilities/class";
import { createGroupUrl } from "@utilities/route";

export const AppSidebar = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");
  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  const authContext = useAuthContext();

  const appState = useAppContext();
  const groupState = useGroupState();

  const [creatingGroup, setCreatingGroup] = createSignal(false);
  const [creatingChannel, setCreatingChannel] = createSignal(false);

  return (
    <>
      <div class="flex flex-row gap-1 sidebar">
        <div class="flex flex-col p-4 pl-0">
          <For each={Object.values(appState.groups)}>
            {(val, index) => {
              const isSelected = () =>
                val.id === groupState.selectedGroup()?.id;
              return (
                <a
                  class={clsx(
                    "listitem p-1 pl-0 flex items-center",
                    isSelected() && "active",
                  )}
                  href={groupState.getGroupUrl(val.id!)}
                >
                  <div class="pill rounded-r-md w-1 transition-all" />
                  <DefaultProfilePicture
                    class="profile m-2"
                    displayName={val.name}
                  />
                </a>
              );
            }}
          </For>
        </div>

        <div class="flex flex-col gap-4 p-4">
          <For each={groupState.channels()}>
            {(val, index) => {
              const isSelected = () =>
                val.id === groupState.selectedChannel()?.id;
              return (
                <div>
                  {val.name} {isSelected() ? "(me)" : ""}
                </div>
              );
            }}
          </For>
        </div>
      </div>
      <div class="flex flex-col mt-auto">
        <Button
          class="block mx-auto text-text-muted m-4"
          onClick={() => setTheme((t) => (t === "dark" ? "light" : "dark"))}
        >
          Toggle theme (current: {theme()})
        </Button>
        <Button onClick={() => setCreatingGroup(true)}>Create Group</Button>
        <Button onClick={() => setCreatingChannel(true)}>Create Channel</Button>
        <Button class="flex mt-auto" onClick={authContext.logout} type="button">
          Logout
        </Button>
      </div>

      <CreateGroupDialog
        open={creatingGroup()}
        onOpenChange={setCreatingGroup}
      />
      <CreateChannelDialog
        open={creatingChannel()}
        onOpenChange={setCreatingChannel}
      />
    </>
  );
};
