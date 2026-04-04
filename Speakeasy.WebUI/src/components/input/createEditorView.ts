import "prosemirror-view/style/prosemirror.css";

import { exampleSetup } from "prosemirror-example-setup";
import { inputRules, InputRule } from "prosemirror-inputrules";
import { schema } from "prosemirror-schema-basic";
import type { MarkType } from "prosemirror-model";
import type { Plugin } from "prosemirror-state";
import { EditorState } from "prosemirror-state";
import type { NodeViewConstructor } from "prosemirror-view";
import { EditorView } from "prosemirror-view";

function markInputRule(pattern: RegExp, markType: MarkType): InputRule {
  return new InputRule(pattern, (state, match, start, end) => {
    const content = match[1];
    return state.tr
      .replaceWith(start, end, schema.text(content || "", [markType.create()]))
      .removeStoredMark(markType);
  });
}

// Patterns must stay in sync with markdownInputRules below
const INLINE_PATTERNS: [RegExp, MarkType][] = [
  [/\*\*([^*]+)\*\*/g, schema.marks.strong],
  [/_([^_]+)_/g, schema.marks.em],
];

function parseInlineText(text: string) {
  const spans: {
    start: number;
    end: number;
    content: string;
    markType: MarkType;
  }[] = [];
  for (const [pattern, markType] of INLINE_PATTERNS) {
    pattern.lastIndex = 0;
    let m: RegExpExecArray | null;
    while ((m = pattern.exec(text)) !== null) {
      spans.push({
        start: m.index,
        end: m.index + m[0].length,
        content: m[1],
        markType,
      });
    }
  }
  spans.sort((a, b) => a.start - b.start);

  const nodes = [];
  let cursor = 0;
  for (const { start, end, content, markType } of spans) {
    if (start < cursor) continue; // overlapping match, skip
    if (start > cursor) nodes.push(schema.text(text.slice(cursor, start)));
    nodes.push(schema.text(content, [markType.create()]));
    cursor = end;
  }
  if (cursor < text.length) nodes.push(schema.text(text.slice(cursor)));
  return nodes;
}

const markdownInputRules = inputRules({
  rules: [
    markInputRule(/\*\*([^*]+)\*\*$/, schema.marks.strong),
    markInputRule(/_([^_]+)_$/, schema.marks.em),
  ],
});

export function createEditorView(
  element: HTMLElement,
  nodeViews: Record<string, NodeViewConstructor>,
  plugins: Plugin[],
  onUpdate?: (state: EditorState) => void,
  initialText?: string,
) {
  let view: EditorView;

  const doc = initialText
    ? schema.node("doc", null, [
        schema.node("paragraph", null, parseInlineText(initialText)),
      ])
    : undefined;

  view = new EditorView(element, {
    state: EditorState.create({
      schema,
      doc,
      plugins: [
        ...exampleSetup({ schema, menuBar: false }),
        markdownInputRules,
        ...plugins,
      ],
    }),
    nodeViews,
    dispatchTransaction(tr) {
      const next = view.state.apply(tr);
      view.updateState(next);
      onUpdate?.(next);
    },
  });

  return view;
}
