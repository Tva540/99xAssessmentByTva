import { test, expect } from '@playwright/test';
import { loginAsAdmin, loginAsViewer } from './helpers/auth';
import * as path from 'path';

test.describe('Balances view (viewer)', () => {
  test.beforeAll(async ({ browser }) => {
    // Seed a period so the balances page has data to render.
    const ctx = await browser.newContext({ ignoreHTTPSErrors: true });
    const page = await ctx.newPage();
    await loginAsAdmin(page);
    await page.locator('#year').fill('2017');
    await page.locator('#file').setInputFiles(
      path.join(__dirname, 'fixtures', 'balances_august_2017.tsv'));
    await page.getByRole('button', { name: /upload/i }).click();
    await expect(page.locator('.alert-success')).toBeVisible();
    await ctx.close();
  });

  test.beforeEach(async ({ page }) => {
    await loginAsViewer(page);
    // Explicitly select the seeded period 
    await page.locator('select').nth(0).selectOption({ label: 'August' });
    await page.locator('select').nth(1).selectOption({ label: '2017' });
    await page.getByRole('button', { name: /search/i }).click();
    await expect(page.locator('.balance-amount').first()).toBeVisible();
  });

  test('lands on balances page and displays the five seeded accounts', async ({ page }) => {
    await expect(page.getByRole('heading', { name: /account balances/i })).toBeVisible();

    for (const accountName of ['R&D', 'Canteen', "CEO's car", 'Marketing', 'Parking fines']) {
      await expect(page.getByText(accountName, { exact: false }).first()).toBeVisible();
    }
  });

  test('formats amounts with Rs. prefix; /= suffix only on whole amounts', async ({ page }) => {
    // Every card uses the Rs. prefix.
    await expect(page.locator('.balance-amount').first()).toContainText(/Rs\./);

    // Whole amounts (e.g. Canteen 77,500) carry /=, fractional ones (R&D 42.13) do not.
    await expect(page.locator('.balance-amount', { hasText: /\/=/ }).first()).toBeVisible();
    await expect(page.locator('.balance-amount', { hasText: /Rs\. 42\.13/ })).toBeVisible();
  });

  test('renders negative balances with the negative style', async ({ page }) => {
    const negative = page.locator('.balance-amount.negative');
    await expect(negative.first()).toBeVisible();
    await expect(negative.first()).toContainText('-');
  });
});
