import { describe, it, expect, beforeEach } from 'vitest';
import { AuthService } from './auth.service';

const STORAGE_KEY = 'x99-auth';

function makeSession(overrides: Partial<{ expiresAt: string; roles: string[] }> = {}) {
  return {
    token: 'abc',
    expiresAt: overrides.expiresAt ?? new Date(Date.now() + 60_000).toISOString(),
    email: 'admin@jondell.local',
    displayName: 'Admin User',
    roles: overrides.roles ?? ['Admin']
  };
}

describe('AuthService', () => {
  beforeEach(() => localStorage.clear());

  it('hydrates an existing session from localStorage', () => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(makeSession()));
    const svc = new AuthService({} as any);

    expect(svc.isAuthenticated()).toBe(true);
    expect(svc.displayName()).toBe('Admin User');
    expect(svc.isAdmin()).toBe(true);
    expect(svc.token()).toBe('abc');
  });

  it('treats an expired session as unauthenticated', () => {
    localStorage.setItem(
      STORAGE_KEY,
      JSON.stringify(makeSession({ expiresAt: new Date(Date.now() - 60_000).toISOString() })));
    const svc = new AuthService({} as any);

    expect(svc.isAuthenticated()).toBe(false);
    expect(svc.token()).toBeNull();
  });

  it('logout clears session and storage', () => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(makeSession()));
    const svc = new AuthService({} as any);

    svc.logout();

    expect(svc.isAuthenticated()).toBe(false);
    expect(svc.session()).toBeNull();
    expect(localStorage.getItem(STORAGE_KEY)).toBeNull();
  });

  it('isAdmin is false for non-admin roles', () => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(makeSession({ roles: ['Viewer'] })));
    const svc = new AuthService({} as any);

    expect(svc.isAdmin()).toBe(false);
  });
});
