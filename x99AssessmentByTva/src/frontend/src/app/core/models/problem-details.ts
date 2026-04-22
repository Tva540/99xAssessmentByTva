export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  [key: string]: unknown;
}

export interface ValidationProblemDetails extends ProblemDetails {
  errors: Record<string, string[]>;
}

export function isValidationProblem(
  value: ProblemDetails | null | undefined
): value is ValidationProblemDetails {
  return !!value && typeof (value as ValidationProblemDetails).errors === 'object';
}
