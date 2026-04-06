/**
 * Combine one or more strings (or conditionals) into a single css class name string
 * @param classes Strings/conditionals to combine
 * @returns A single string which can be used as a class input
 * @example
 * <div class={clsx('bg-white', isEnabled && 'bg-red')} />
 */
export function clsx(
  ...classes: (string | null | undefined | false)[]
): string {
  return classes.filter((c) => !!c).join(" ");
}
