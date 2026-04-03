import { JSX, ParentComponent } from "solid-js";

export const Label: ParentComponent<
  JSX.LabelHTMLAttributes<HTMLLabelElement>
> = (props) => {
  return <label {...props} />;
};
