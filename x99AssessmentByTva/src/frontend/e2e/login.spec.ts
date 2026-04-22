import { test, expect } from '@playwright/test';
import { loginAsAdmin, loginAsViewer } from './helpers/auth';

test.describe('Login', () => {
  test('admin login redirects to Upload', async ({ page }) => {
    await loginAsAdmin(page);
    await expect(page.getByRole('heading', { name: /upload monthly balances/i })).toBeVisible();
  });

  test('viewer login redirects to Balances', async ({ page }) => {
    await loginAsViewer(page);
    await expect(page.getByRole('heading', { name: /account balances/i })).toBeVisible();
  });

  test('shows error on invalid credentials', async ({ page }) => {
    await page.goto('/login');
    await page.locator('#email').fill('nobody@jondell.local');
    await page.locator('#password').fill('WrongPassword1!');
    await page.getByRole('button', { name: /sign in/i }).click();

    await expect(page.locator('.alert-danger')).toBeVisible();
    await expect(page).toHaveURL(/\/login$/);
  });

  test('keeps submit disabled while form is invalid', async ({ page }) => {
    await page.goto('/login');
    const submit = page.getByRole('button', { name: /sign in/i });
    await expect(submit).toBeDisabled();

    await page.locator('#email').fill('not-an-email');
    await page.locator('#password').fill('anything');
    await expect(submit).toBeDisabled();
  });
});
