import { Component, createEffect, onCleanup, type JSX } from "solid-js";
import { schema } from "prosemirror-schema-basic";
import type { EditorView } from "prosemirror-view";
import { EditorState } from "prosemirror-state";

import { createEditorView } from "./createEditorView";

export type SlateEditorProps = Omit<
  JSX.HTMLAttributes<HTMLDivElement>,
  "onChange"
> & {
  onChange: (newValue: string) => void;
  value: string;
};

export const SlateEditor: Component<SlateEditorProps> = (props) => {
  let view: EditorView | undefined;

  const editorRef = (el: HTMLDivElement) => {
    view = createEditorView(
      el,
      {},
      [],
      (state) => {
        props.onChange(state.doc.textContent);
      },
      props.value || undefined,
    );
    onCleanup(() => view?.destroy());
  };

  createEffect(() => {
    if (props.value === "" && view) {
      view.updateState(
        EditorState.create({ schema, plugins: view.state.plugins }),
      );
    }
  });

  return (
    <div class={`flex flex-col flex-1 ${props.class ?? ""}`}>
      <div ref={editorRef} class="flex-1 p-2" />
    </div>
  );
};
