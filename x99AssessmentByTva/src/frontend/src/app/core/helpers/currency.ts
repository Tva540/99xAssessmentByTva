/**
 * Formats an amount as Sri Lankan Rupees per local convention:
 *
 *   Whole amounts:  "Rs. 98,000/="    — the /= suffix indicates exact, zero cents.
 *   With cents:     "Rs. 10.56"       — no /= when a fractional part is present.
 *   Negatives:      "Rs. -19,112/="   — the sign precedes the number.
 *   Missing data:   "—"               — distinguishes "no value" from "zero".
 */
export function formatRupee(amount: number | null | undefined): string {
  if (amount === null || amount === undefined) return '—';

  const abs = Math.abs(amount);
  const sign = amount < 0 ? '-' : '';
  const hasCents = abs % 1 !== 0;

  if (hasCents) {
    const n = abs.toLocaleString('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
    return `Rs. ${sign}${n}`;
  }

  const n = abs.toLocaleString('en-US', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  });
  return `Rs. ${sign}${n}/=`;
}

export function isNegativeAmount(amount: number | null | undefined): boolean {
  return amount !== null && amount !== undefined && amount < 0;
}
