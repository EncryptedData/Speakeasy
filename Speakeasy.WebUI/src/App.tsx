import { createEffect, createSignal, type Component } from "solid-js";

import { Chat } from "@components/chat/Chat";
import { useAuthContext } from "@context/authContext";
import { useNavigate } from "@solidjs/router";
import { Button } from "@components/input/button";
import { TextField } from "@components/input/textField";
import { Label } from "@components/input/label";

const App: Component = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");

  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  const { isLoggedIn, logout } = useAuthContext();
  const navigate = useNavigate();
  createEffect(() => {
    if (!isLoggedIn()) {
      navigate("/login", { replace: true });
    }
  });

  // TODO: Wire up groupIds once we can create & retrieve them
  const [channelId, setChannelId] = createSignal("");

  return (
    <div class="flex flex-1">
      <div class="flex-1 flex flex-col">
        <p class="text-4xl text-accent text-center py-20">
          Hello tailwind; Sup!!
        </p>

        <Button
          class="block mx-auto text-text-muted m-4"
          onClick={() => setTheme((t) => (t === "dark" ? "light" : "dark"))}
        >
          Toggle theme (current: {theme()})
        </Button>
        <Label>
          ChannelId
          <TextField
            value={channelId()}
            onChange={(e) => setChannelId(e.currentTarget.value)}
          />
        </Label>
        <Button class="flex mt-auto" onClick={logout} type="button">
          Logout
        </Button>
      </div>
      <div class="flex flex-4">
        <Chat channelId={channelId} />
      </div>
    </div>
  );
};

export default App;
