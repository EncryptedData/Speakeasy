import { createEffect, createSignal, type Component } from "solid-js";
import { Sidebar } from "./components/sidebar/Sidebar";

const App: Component = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");

  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  return (
    <div>
      <Sidebar/>
    </div>
  );
};

export default App;
