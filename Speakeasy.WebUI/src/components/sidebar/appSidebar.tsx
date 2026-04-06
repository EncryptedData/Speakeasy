import "./appSidebar.css";

import { createSignal, For } from "solid-js";

import { CreateChannelDialog } from "@components/channel/createChannelDialog";
import { CreateGroupDialog } from "@components/group/createGroupDialog";
import { Button } from "@components/input/button";
import { useAuthContext } from "@context/authContext";
import { useGroupState } from "@hooks/group";
import { useAppContext } from "@context/appContext";
import { DefaultProfilePicture } from "@components/chat/defaultProfilePicture";
import { clsx } from "@utilities/class";

export const AppSidebar = () => {
  const authContext = useAuthContext();

  const appState = useAppContext();
  const groupState = useGroupState();

  const [creatingGroup, setCreatingGroup] = createSignal(false);
  const [creatingChannel, setCreatingChannel] = createSignal(false);

  return (
    <>
      <div class="flex flex-1 flex-row gap-1 sidebar">
        <div class="flex flex-col pl-0 bg-gray-800 groups">
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

        <div class="flex flex-1 flex-col gap-2 p-4 channels">
          <div class="font-bold">{groupState.selectedGroup()?.name}</div>
          <div class="border-b w-full" />
          <For each={groupState.channels()}>
            {(val, index) => {
              const isSelected = () =>
                val.id === groupState.selectedChannel()?.id;
              return (
                <a
                  class={clsx(
                    "listitem p-1 pl-4 flex items-center rounded-lg hover:bg-white/5 transition-all text-text-muted",
                    isSelected() &&
                      "active bg-white/10 hover:bg-white/10 font-white text-text-primary font-medium",
                  )}
                  href={groupState.getGroupUrl(
                    groupState.selectedGroup()?.id!,
                    val.id!,
                  )}
                >
                  {val.name}
                </a>
              );
            }}
          </For>
        </div>
      </div>
      <div class="flex flex-col mt-auto">
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
