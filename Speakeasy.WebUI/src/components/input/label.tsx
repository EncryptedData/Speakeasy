import { JSX, ParentComponent } from "solid-js";

export const Label: ParentComponent<
  JSX.LabelHTMLAttributes<HTMLLabelElement>
> = (props) => {
  return (
    <label {...props} class={`flex gap-1 flex-col ${props.class || ""}`} />
  );
};
