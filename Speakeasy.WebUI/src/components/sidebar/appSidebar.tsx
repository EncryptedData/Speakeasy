import "./appSidebar.css";

import { createSelector, createSignal, For } from "solid-js";

import { CreateChannelDialog } from "@components/channel/createChannelDialog";
import { DefaultProfilePicture } from "@components/chat/defaultProfilePicture";
import { CreateGroupDialog } from "@components/group/createGroupDialog";
import { Button } from "@components/input/button";
import { useAppContext } from "@context/appContext";
import { useAuthContext } from "@context/authContext";
import { useGroupState } from "@hooks/group";
import { clsx } from "@utilities/class";
import { Tooltip } from "@components/tooltip/tooltip";

export const AppSidebar = () => {
  const authContext = useAuthContext();

  const appState = useAppContext();
  const groupState = useGroupState();

  const [creatingGroup, setCreatingGroup] = createSignal(false);
  const [creatingChannel, setCreatingChannel] = createSignal(false);

  const isGroupSelected = createSelector(
    groupState.selectedGroup,
    (id: string, selectedGroup) => selectedGroup?.id === id,
  );

  const isChannelSelected = createSelector(
    groupState.selectedChannel,
    (id: string, selectedChannel) => selectedChannel?.id === id,
  );

  return (
    <>
      <div class="flex flex-1 flex-row gap-1 sidebar">
        <div class="flex flex-col pl-0 bg-gray-800 groups">
          <For each={Object.values(appState.groups)}>
            {(val, index) => {
              return (
                <Tooltip
                  content={<div>{val.name}</div>}
                  openDelay={0}
                  placement="right"
                >
                  <a
                    class={clsx(
                      "listitem flex items-center",
                      isGroupSelected(val.id!) && "active",
                    )}
                    href={groupState.getGroupUrl(val.id!)}
                  >
                    <div class="pill rounded-r-md w-1 transition-all" />
                    <DefaultProfilePicture
                      class="profile m-2"
                      displayName={val.name}
                    />
                  </a>
                </Tooltip>
              );
            }}
          </For>
          <button
            class={clsx(
              "listitem flex items-center justify-center rounded-full bg-red cursor-pointer",
            )}
            onClick={() => setCreatingGroup(true)}
            title="Create a Group"
          >
            <div class="profile flex items-center justify-center rounded-full bg-white/20 hover:bg-bg-base transition-all">
              +
            </div>
          </button>
        </div>

        <div class="flex flex-1 flex-col gap-2 p-4 channels">
          <div class={"flex gap-1"}>
            <div class="font-bold">{groupState.selectedGroup()?.name}</div>
            <button
              class="ml-auto min-w-8 min-h-8 w-8 h-8 cursor-pointer p-1 rounded-full hover:bg-bg-surface-hover active:bg-bg-elevated-hover transition-colors"
              onClick={() => setCreatingChannel(true)}
            >
              +
            </button>
          </div>
          <div class="border-b w-full" />
          <For each={groupState.channels()}>
            {(val, index) => {
              return (
                <a
                  class={clsx(
                    "listitem p-1 pl-4 flex items-center rounded-lg hover:bg-white/5 transition-all text-text-muted",
                    isChannelSelected(val.id!) &&
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
