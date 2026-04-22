import { Component, OnInit, computed, signal } from '@angular/core';
import { BalanceService } from '../../core/services/balance.service';
import { BalancePeriod, MonthlyBalances } from '../../core/models/balance.models';
import { formatRupee } from '../../core/helpers/currency';
import { extractProblemMessage } from '../../core/helpers/problem-details';

const MONTH_NAMES = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'
];

@Component({
  selector: 'app-balances',
  standalone: false,
  templateUrl: './balances.html'
})
export class Balances implements OnInit {
  readonly periods = signal<BalancePeriod[]>([]);
  readonly monthly = signal<MonthlyBalances | null>(null);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly months = MONTH_NAMES.map((name, i) => ({ value: i + 1, name }));
  readonly years = computed(() =>
    [...new Set(this.periods().map(p => p.year))].sort((a, b) => b - a));

  selectedYear: number | null = null;
  selectedMonth: number | null = null;

  constructor(private readonly balanceService: BalanceService) {}

  ngOnInit(): void {
    this.balanceService.getPeriods().subscribe({
      next: periods => {
        this.periods.set(periods);
        if (periods.length > 0) {
          this.selectedYear = periods[0].year;
          this.selectedMonth = periods[0].month;
          this.loadMonthly();
        }
      },
      error: err => this.errorMessage.set(extractProblemMessage(err, 'Failed to load periods'))
    });
  }

  onSearch(): void {
    this.loadMonthly();
  }

  formatAmount(amount: number | null): string {
    return formatRupee(amount);
  }

  private loadMonthly(): void {
    if (this.selectedYear === null || this.selectedMonth === null) return;
    this.loading.set(true);
    this.errorMessage.set(null);

    this.balanceService.getMonthly(this.selectedYear, this.selectedMonth).subscribe({
      next: data => { this.monthly.set(data); this.loading.set(false); },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set(extractProblemMessage(err, 'Failed to load balances'));
      }
    });
  }
}
