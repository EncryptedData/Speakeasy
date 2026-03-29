import { createEffect, createSignal, Show, type Component } from "solid-js";
import { Portal } from "solid-js/web";

import { Chat } from "@components/chat/Chat";
import { LoginForm } from "@components/auth/loginForm";
import { useAuthContext } from "@context/authContext";

const App: Component = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");

  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  const { isLoggedIn, authLoading } = useAuthContext();

  return (
    <>
      <Portal>
        <Show when={!authLoading() && !isLoggedIn()}>
          <LoginForm />
        </Show>
      </Portal>
      <div class="flex flex-1">
        <div class="flex-1">
          <p class="text-4xl text-accent text-center py-20">
            Hello tailwind; Sup!!
          </p>

          <button
            class="block mx-auto text-text-muted"
            onClick={() => setTheme((t) => (t === "dark" ? "light" : "dark"))}
          >
            Toggle theme (current: {theme()})
          </button>
        </div>
        <div class="flex flex-4">
          <Chat />
        </div>
      </div>
    </>
  );
};

export default App;
