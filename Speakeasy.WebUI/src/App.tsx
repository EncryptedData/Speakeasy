import { Root, Panel, Handle } from "@corvu/resizable";
import { makePersisted } from "@solid-primitives/storage";
import { createEffect, createSignal, type Component } from "solid-js";

import { AppSidebar } from "@components/sidebar/appSidebar";
import { useAppContext } from "@context/appContext";
import { useAuthContext } from "@context/authContext";
import { useNavigate, useParams } from "@solidjs/router";
import {
  getCurrentChannelId,
  getCurrentGroupId,
  navigateToGroup,
} from "@utilities/route";
import { Chat } from "@components/chat/Chat";

const App: Component = () => {
  const params = useParams();

  const [sizes, setSizes] = makePersisted(createSignal([]), {
    name: "resizable-sizes",
  });

  const appContext = useAppContext();

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

  const channelId = () => getCurrentChannelId(params);

  return (
    <Root sizes={sizes()} onSizesChange={setSizes} class="size-full">
      <Panel
        initialSize={0.2}
        minSize={0.2}
        maxSize={0.4}
        class="flex flex-col"
      >
        <AppSidebar />
      </Panel>
      <Handle aria-label="Resize Handle" class="group basis-3 px-0.75">
        <div class="size-full rounded-sm transition-colors group-data-active:bg-corvu-300 group-data-dragging:bg-corvu-100" />
      </Handle>
      <Panel initialSize={0.8} class="rounded-lg bg-corvu-100">
        <Chat channelId={channelId} />
      </Panel>
    </Root>
  );
};

export default App;
