export function parseDateFromUuid(uuid: string): Date {
  return new Date(parseInt(uuid.substring(0, 12), 16));
}
