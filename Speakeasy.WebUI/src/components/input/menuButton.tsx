import { Component, type JSX } from "solid-js";

import { clsx } from "@utilities/class";

export const MenuButton: Component<
  JSX.ButtonHTMLAttributes<HTMLButtonElement>
> = (props) => {
  return (
    <button
      {...props}
      class={clsx(
        "px-4 py-1 flex items-center rounded-lg hover:bg-white/5 transition-all cursor-pointer",
        props.class,
      )}
    />
  );
};
