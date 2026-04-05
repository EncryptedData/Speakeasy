import { Component } from "solid-js";

const COLORS = [
  "66C5CC",
  "F6CF71",
  "F89C74",
  "DCB0F2",
  "87C55F",
  "9EB9F3",
  "FE88B1",
  "C9DB74",
  "8BEOA4",
  "B497E7",
  "B3B3B3",
];

export type DefaultProfilePictureProps = {
  class?: string;
  displayName: string;
};

export const DefaultProfilePicture: Component<DefaultProfilePictureProps> = (
  props,
) => {
  const split = props.displayName.split(" ", 2);
  let letters = split[0]![0]!;
  if (split.length === 2) {
    letters += split[1]![0]!;
  }

  const color = COLORS[getColorIndex(props.displayName)];

  return (
    <div
      class={`uppercase rounded-full flex justify-center items-center ${props.class || ""}`}
      style={{ "background-color": `#${color}` }}
    >
      {letters}
    </div>
  );
};

function getColorIndex(name: string): number {
  let sum = 0;
  for (const char of name) {
    sum += char.charCodeAt(0);
  }

  return sum % COLORS.length;
}
