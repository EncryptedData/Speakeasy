import { ValidationError } from "@models/validationError";

/**
 * Attempt to extract the first error for each field by case insensitive name matching
 * @param response ValidationError from the server.
 * @param fields Fields to extract errors for.
 * @returns An object containing the first matching error.
 */
export function getErrorMessages<K extends string>(
  validationError: ValidationError,
  fields: K[],
): Partial<Record<K, string>> {
  const output: Partial<Record<K, string>> = {};

  for (const field of fields) {
    if (output[field]) {
      continue;
    }

    for (const [errorCode, errorMessages] of Object.entries(
      validationError.errors || {},
    )) {
      if (errorCode.toLowerCase().includes(field.toLowerCase())) {
        output[field] = errorMessages[0]!;
      }
    }
  }

  return output;
}
