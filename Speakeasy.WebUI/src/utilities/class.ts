export function clsx(...classes: (string | false)[]): string {
  return classes.filter((c) => !!c).join(" ");
}
