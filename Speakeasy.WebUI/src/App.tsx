import { createEffect, createSignal, type Component } from "solid-js";
import { Sidebar } from "./components/sidebar/Sidebar";

const App: Component = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");

  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  return (
    <div>
      <Sidebar />
      <button
        class="block mx-auto text-text-muted"
        onClick={() => setTheme(t => t === "dark" ? "light" : "dark")}
      >
        Toggle theme (current: {theme()})
      </button>
    </div>
  );
};

export default App;
