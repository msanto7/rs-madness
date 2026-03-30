import type { AppApiError } from './client';

export function isAppApiError(value: unknown): value is AppApiError {
  if (typeof value !== 'object' || value === null) return false;
  const v = value as Record<string, unknown>;
  return (
    typeof v.status === 'number' &&
    typeof v.message === 'string' &&
    Array.isArray(v.errors)
  );
}

export function getApiErrorStatus(value: unknown): number | null {
  return isAppApiError(value) ? value.status : null;
}

export function getApiErrorMessages(value: unknown, fallback: string): string[] {
  if (!isAppApiError(value)) return [fallback];
  if (value.errors.length > 0) return value.errors;
  return [value.message || fallback];
}