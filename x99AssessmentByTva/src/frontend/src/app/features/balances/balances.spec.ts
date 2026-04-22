import { describe, it, expect } from 'vitest';
import { formatRupee } from '../../core/helpers/currency';

describe('formatRupee', () => {
  it('whole amounts get the /= suffix with no decimals', () => {
    expect(formatRupee(98000)).toBe('Rs. 98,000/=');
    expect(formatRupee(2000)).toBe('Rs. 2,000/=');
  });

  it('amounts with cents omit the /= suffix and show two decimals', () => {
    expect(formatRupee(10.56)).toBe('Rs. 10.56');
    expect(formatRupee(5.63)).toBe('Rs. 5.63');
  });

  it('negative whole amounts keep the /= suffix with sign before the digits', () => {
    expect(formatRupee(-19112)).toBe('Rs. -19,112/=');
    expect(formatRupee(-600)).toBe('Rs. -600/=');
  });

  it('negative amounts with cents omit the /= suffix', () => {
    expect(formatRupee(-12.34)).toBe('Rs. -12.34');
  });

  it('returns a dash for null or undefined', () => {
    expect(formatRupee(null)).toBe('—');
    expect(formatRupee(undefined)).toBe('—');
  });
});
