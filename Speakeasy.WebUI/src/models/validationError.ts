export type ValidationError = {
  /**
   * e.g. "https://tools.ietf.org/html/rfc9110#section-15.5.1"
   */
  type: string;

  /**
   * e.g. "One or more validation errors occurred."
   */
  title: string;

  status: number;

  /**
   * ErrorCode -> ErrorMessage[]
   * e.g. PasswordTooShort: ["Password must be at least 6 characters"]
   */
  errors: Record<string, string[]>;
};
