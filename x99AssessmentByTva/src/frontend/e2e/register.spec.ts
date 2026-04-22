import { test, expect } from '@playwright/test';

test.describe('Register', () => {
  test('creates a new viewer account and redirects to login', async ({ page }) => {
    const uniqueEmail = `viewer.${Date.now()}@jondell.local`;

    await page.goto('/register');
    await page.locator('#displayName').fill('E2E Viewer');
    await page.locator('#email').fill(uniqueEmail);
    await page.locator('#password').fill('Passw0rd!');
    await page.getByRole('button', { name: /create account|register|sign up/i }).click();

    await expect(page.locator('.alert-success')).toBeVisible();
    await expect(page.locator('.alert-success')).toContainText(uniqueEmail);
    await expect(page).toHaveURL(/\/login$/, { timeout: 5000 });
  });

  test('rejects duplicate registration', async ({ page }) => {
    await page.goto('/register');
    await page.locator('#displayName').fill('Duplicate Admin');
    await page.locator('#email').fill('admin@jondell.local');
    await page.locator('#password').fill('Passw0rd!');
    await page.getByRole('button', { name: /create account|register|sign up/i }).click();

    await expect(page.locator('.alert-danger')).toBeVisible();
  });

  test('keeps submit disabled when password too short', async ({ page }) => {
    await page.goto('/register');
    await page.locator('#displayName').fill('Short Pw User');
    await page.locator('#email').fill('short.pw@jondell.local');
    await page.locator('#password').fill('short');

    const submit = page.getByRole('button', { name: /create account|register|sign up/i });
    await expect(submit).toBeDisabled();
  });
});
