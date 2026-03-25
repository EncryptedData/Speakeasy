import { Component, createEffect, createSignal, type JSX } from "solid-js";

import { ProsemirrorAdapterProvider } from "@prosemirror-adapter/solid";

export type SlateEditorProps = Omit<
  JSX.HTMLAttributes<HTMLDivElement>,
  "onChange"
> & {
  onChange: (newValue: string) => void;
  value: string;
};

export const SlateEditor: Component<SlateEditorProps> = (props) => {
  const [container, setContainer] = createSignal<HTMLDivElement>();
  // const editor = createTiptapEditor(() => ({
  //   editorProps: {
  //     attributes: {
  //       class: `chat__input p-4 bg-bg-input rounded-sm ${props.class || ""}`,
  //     },
  //   },
  //   element: container()!,
  //   extensions: [
  //     StarterKit,
  //     Link.configure({
  //       openOnClick: false,
  //       HTMLAttributes: {
  //         rel: "noopener noreferrer",
  //         target: null,
  //       },
  //     }),
  //     Strike,
  //   ],
  //   content: `XD! ~~oopsie daisies~~`,
  // }));

  // const json = useEditorJSON(editor);
  // createEffect(() => {
  //   console.log(JSON.stringify(json(), null, 2));
  // });

  return (
    <ProsemirrorAdapterProvider>
      <div class={"flex-1"} id="editor" ref={(r) => setContainer(r)} />
    </ProsemirrorAdapterProvider>
  );
};
