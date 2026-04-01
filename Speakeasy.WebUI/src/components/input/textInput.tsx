import { Component, JSX } from "solid-js";

export type TextFieldProps = JSX.InputHTMLAttributes<HTMLInputElement>;

export const TextInput: Component<TextFieldProps> = (props) => {
  return (
    <input
      {...props}
      class={`p-4 bg-bg-input ${props.class || ""} ${props["aria-invalid"] ? "border border-solid border-error" : ""}`}
    />
  );
};
