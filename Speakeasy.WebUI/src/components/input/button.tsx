import { Component, JSX } from "solid-js";

export const Button: Component<JSX.ButtonHTMLAttributes<HTMLButtonElement>> = (
  props,
) => {
  return (
    <button
      {...props}
      class={`p-4 rounded-md bg-bg-surface hover:bg-bg-surface-hover active:bg-bg-elevated-hover transition-colors ${props.class}`}
    />
  );
};
