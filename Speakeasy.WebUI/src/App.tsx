import { createEffect, createSignal, type Component } from "solid-js";
import { makePersisted } from '@solid-primitives/storage'
import { Sidebar } from "./components/sidebar/Sidebar";
import Resizable from 'corvu/resizable'

const App: Component = () => {
  const [theme, setTheme] = createSignal<"dark" | "light">("dark");
  const [sizes, setSizes] = makePersisted(createSignal<number[]>([]), {
    name: 'resizable-sizes'
  })
  const [collapsed, setCollapse] = makePersisted(createSignal<boolean>(true), {
    name: 'sidebar-collapse-state'
  })
  createEffect(() => {
    document.documentElement.dataset.theme = theme();
  });

  return (
    <Resizable sizes={sizes()} onSizesChange={setSizes} orientation="horizontal" class="h-full rounded-sm">
      <Resizable.Panel
        initialSize={0.3}
        minSize={0.25}
        collapsible onCollapse={setCollapse}
        collapsedSize={0.01}
        maxSize={0.4}
        class="flex flex-col justify-end">
        <Sidebar collapsed={collapsed()} />
      </Resizable.Panel>
      <Resizable.Handle aria-label="Sidebar resize handle"
        class="group basis-3 px-0.75">
        <div class="h-full w-1 transition-colors group-data-active:bg-corvu-300 bg-corvu-200 group-data-dragging:bg-corvu-100" />
      </Resizable.Handle>
      <Resizable.Panel
        initialSize={0.75}
        class="flex flex-col items-center justify-center space-y-2 overflow-hidden px-4">
        <div>
          <button class="block mx-auto text-text-muted" onClick={() => setTheme(t => t === "dark" ? "light" : "dark")}>
            Toggle theme (current: {theme()})
          </button>
        </div>
      </Resizable.Panel>
    </Resizable>
  );
};

export default App;