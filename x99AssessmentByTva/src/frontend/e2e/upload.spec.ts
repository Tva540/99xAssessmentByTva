import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './helpers/auth';
import * as path from 'path';

test.describe('Upload (admin)', () => {
  test.beforeEach(async ({ page }) => {
    await loginAsAdmin(page);
  });

  test('uploads a TSV file and shows the import result', async ({ page }) => {
    const year = 2017;
    await page.locator('#year').fill(String(year));
    await page.locator('#file').setInputFiles(
      path.join(__dirname, 'fixtures', 'balances_august_2017.tsv')
    );

    await page.getByRole('button', { name: /upload/i }).click();

    await expect(page.locator('.alert-success')).toBeVisible();
    await expect(page.locator('.alert-success')).toContainText('Imported 5 rows');
    await expect(page.locator('.alert-success')).toContainText(`8/${year}`);
  });

  test('keeps submit disabled until a file is selected', async ({ page }) => {
    const submit = page.getByRole('button', { name: /upload/i });
    await expect(submit).toBeDisabled();
  });
});
