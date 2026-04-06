import { createEffect, createSignal } from "solid-js";

import { CreateChannelDialog } from "@components/channel/createChannelDialog";
import { CreateGroupDialog } from "@components/group/createGroupDialog";
import { Button } from "@components/input/button";
import { useGroupState } from "@hooks/group";
import { useAuthContext } from "@context/authContext";

export const AppSidebar = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");
  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  const authContext = useAuthContext();

  const groupState = useGroupState();

  const [creatingGroup, setCreatingGroup] = createSignal(false);
  const [creatingChannel, setCreatingChannel] = createSignal(false);

  return (
    <>
      <p class="text-4xl text-accent text-center py-20">
        Hello tailwind; Sup!!
      </p>
      <p class="text-md text-accent text-center py-20">
        {groupState.selectedGroup?.id || ""}
      </p>

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
