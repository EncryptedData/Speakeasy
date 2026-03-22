import { createEffect, createSignal, type Component } from "solid-js";
import { Chat } from "./components/chat/Chat";

const App: Component = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");

  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  return (
    <div>
      <p class="text-4xl text-accent text-center py-20">
        Hello tailwind; Sup!!
      </p>
      <Chat />
      <button
        class="block mx-auto text-text-muted"
        onClick={() => setTheme((t) => (t === "dark" ? "light" : "dark"))}
      >
        Toggle theme (current: {theme()})
      </button>
    </div>
  );
};

export default App;
