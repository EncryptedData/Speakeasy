import { createEffect, createSignal, type Component } from "solid-js";

import { Chat } from "@components/chat/Chat";
import { useAuthContext } from "@context/authContext";
import { useNavigate, useParams } from "@solidjs/router";
import { Button } from "@components/input/button";
import { CreateGroupDialog } from "@components/group/createGroupDialog";
import { useGroupState } from "@hooks/group";
import { CreateChannelDialog } from "@components/channel/createChannelDialog";
import {
  getCurrentChannelId,
  getCurrentGroupId,
  navigateToGroup,
} from "@utilities/route";
import { useAppContext } from "@context/appContext";

const App: Component = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");
  const params = useParams();

  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  const appContext = useAppContext();
  const groupState = useGroupState();

  const authContext = useAuthContext();
  const navigate = useNavigate();
  createEffect(() => {
    if (!authContext.isLoggedIn()) {
      navigate("/login", { replace: true });
    }

    const firstGroup = Object.keys(appContext.groups)[0];
    if (!getCurrentGroupId(params) && firstGroup) {
      navigateToGroup(
        navigate,
        firstGroup,
        appContext.groups[firstGroup]?.channels?.[0],
      );
    }
  });

  const channelId = getCurrentChannelId(params);

  const [creatingGroup, setCreatingGroup] = createSignal(false);
  const [creatingChannel, setCreatingChannel] = createSignal(false);

  return (
    <div class="flex flex-1">
      <div class="flex-1 flex flex-col">
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
      </div>
      <div class="flex flex-4">
        <Chat channelId={() => channelId || ""} />
      </div>

      <CreateGroupDialog
        open={creatingGroup()}
        onOpenChange={setCreatingGroup}
      />
      <CreateChannelDialog
        open={creatingChannel()}
        onOpenChange={setCreatingChannel}
      />
    </div>
  );
};

export default App;
