import { HttpErrorResponse } from '@angular/common/http';
import { ProblemDetails, isValidationProblem } from '../models/problem-details';

export function extractProblemMessage(err: unknown, fallback: string): string {
  const body = toProblem(err);
  if (!body) return fallback;

  if (isValidationProblem(body)) {
    const messages = Object.values(body.errors).flat();
    if (messages.length > 0) return messages.join(' ');
  }

  return body.detail ?? body.title ?? fallback;
}

function toProblem(err: unknown): ProblemDetails | null {
  if (err instanceof HttpErrorResponse) {
    return (err.error && typeof err.error === 'object')
      ? err.error as ProblemDetails
      : null;
  }
  const maybe = err as { error?: unknown } | null;
  return (maybe?.error && typeof maybe.error === 'object')
    ? maybe.error as ProblemDetails
    : null;
}
