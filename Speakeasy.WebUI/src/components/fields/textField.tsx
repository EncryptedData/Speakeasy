import { Component, JSX, splitProps } from "solid-js";

import { Label } from "@components/input/label";
import { TextInput } from "@components/input/textInput";

export type TextFieldProps = {
  name: string;
  type?: "text" | "email" | "tel" | "password" | "url" | "date";
  label?: string;
  placeholder?: string;
  value: string | undefined;
  error: string;
  required?: boolean;
  ref: (element: HTMLInputElement) => void;
  onInput: JSX.EventHandler<HTMLInputElement, InputEvent>;
  onChange: JSX.EventHandler<HTMLInputElement, Event>;
  onBlur: JSX.EventHandler<HTMLInputElement, FocusEvent>;
};

export const TextField: Component<TextFieldProps> = (props) => {
  const [, inputProps] = splitProps(props, ["value", "label", "error"]);

  return (
    <div class="field-wrapper flex gap-1 flex-col">
      {!!props.label && <Label for={inputProps.name}>{props.label}</Label>}
      <TextInput
        {...inputProps}
        type={inputProps.type || "text"}
        id={props.name}
        value={props.value || ""}
        aria-invalid={!!props.error}
        aria-errormessage={`${props.name}-error`}
      />
      {!!props.error && (
        <div id={`${props.name}-error`} class="text-error">
          {props.error}
        </div>
      )}
    </div>
  );
};
