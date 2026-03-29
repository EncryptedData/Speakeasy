import { createEffect, createSignal, Show, type Component } from "solid-js";

import { Chat } from "@components/chat/Chat";
import { useAuthContext } from "@context/authContext";
import { redirect, useNavigate } from "@solidjs/router";
import { Button } from "@components/input/button";

const App: Component = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");

  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  const { isLoggedIn, authLoading, logout } = useAuthContext();
  const navigate = useNavigate();
  createEffect(() => {
    // TODO: should probably have loading indicator or something idk
    if (!authLoading() && !isLoggedIn()) {
      navigate("/login", {
        replace: true,
      });
    }
  });

  return (
    <div class="flex flex-1">
      <div class="flex-1 flex flex-col">
        <p class="text-4xl text-accent text-center py-20">
          Hello tailwind; Sup!!
        </p>

        <button
          class="block mx-auto text-text-muted"
          onClick={() => setTheme((t) => (t === "dark" ? "light" : "dark"))}
        >
          Toggle theme (current: {theme()})
        </button>
        <Button class="flex mt-auto" onClick={logout} type="button">
          Logout
        </Button>
      </div>
      <div class="flex flex-4">
        <Chat />
      </div>
    </div>
  );
};

export default App;
