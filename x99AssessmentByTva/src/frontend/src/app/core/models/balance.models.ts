export interface Account {
  id: number;
  accountTypeCode: number;
  name: string;
  displayOrder: number;
}

export interface AccountBalance {
  accountId: number;
  accountTypeCode: number;
  name: string;
  displayOrder: number;
  amount: number | null;
}

export interface MonthlyBalances {
  year: number;
  month: number;
  periodLabel: string;
  balances: AccountBalance[];
}

export interface BalancePeriod {
  year: number;
  month: number;
  label: string;
}

export interface UploadResult {
  year: number;
  month: number;
  importedCount: number;
  warnings: string[];
}

export interface AnnualAccountColumn {
  accountId: number;
  name: string;
  displayOrder: number;
  annualTotal: number;
}

export interface AnnualMonthRow {
  month: number;
  monthLabel: string;
  amounts: (number | null)[];
  monthTotal: number;
}

export interface AnnualSummary {
  year: number;
  accounts: AnnualAccountColumn[];
  months: AnnualMonthRow[];
  grandTotal: number;
}
