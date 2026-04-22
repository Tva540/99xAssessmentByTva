import { Page, expect } from '@playwright/test';

export const ADMIN_EMAIL = 'admin@jondell.local';
export const ADMIN_PASSWORD = 'Admin@12345';
export const VIEWER_EMAIL = 'viewer@jondell.local';
export const VIEWER_PASSWORD = 'Viewer@12345';

export async function loginAsAdmin(page: Page) {
  await page.goto('/login');
  await page.locator('#email').fill(ADMIN_EMAIL);
  await page.locator('#password').fill(ADMIN_PASSWORD);
  await page.getByRole('button', { name: /sign in/i }).click();
  await expect(page).toHaveURL(/\/upload$/);
}

export async function loginAsViewer(page: Page) {
  await page.goto('/login');
  await page.locator('#email').fill(VIEWER_EMAIL);
  await page.locator('#password').fill(VIEWER_PASSWORD);
  await page.getByRole('button', { name: /sign in/i }).click();
  await expect(page).toHaveURL(/\/balances$/);
}
