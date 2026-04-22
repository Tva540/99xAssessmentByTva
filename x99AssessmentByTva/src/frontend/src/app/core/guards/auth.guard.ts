import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isAuthenticated()) return true;
  router.navigate(['/login']);
  return false;
};

//INFO: Admin-only. Non-admins are pushed to their own landing page.
export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isAuthenticated() && auth.isAdmin()) return true;
  router.navigate(['/balances']);
  return false;
};

//INFO: Viewer-only (non-admin). The PDF states admins can *only* upload and
//      view reports — so admins are blocked from the balances page.
export const viewerGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isAuthenticated() && !auth.isAdmin()) return true;
  router.navigate(['/upload']);
  return false;
};

//INFO: Role-based landing page: admins go to /upload, viewers to /balances,
//      anonymous visitors to /login. Used on the root path ('').
export const rootRedirect: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isAuthenticated()) {
    router.navigate(['/login']);
    return false;
  }
  router.navigate([auth.isAdmin() ? '/upload' : '/balances']);
  return false;
};
