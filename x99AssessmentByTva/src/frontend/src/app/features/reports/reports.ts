import { Component, OnInit, computed, signal } from '@angular/core';
import { BalanceService } from '../../core/services/balance.service';
import { AnnualSummary, BalancePeriod } from '../../core/models/balance.models';
import { formatRupee, isNegativeAmount } from '../../core/helpers/currency';
import { extractProblemMessage } from '../../core/helpers/problem-details';

@Component({
  selector: 'app-reports',
  standalone: false,
  templateUrl: './reports.html'
})
export class Reports implements OnInit {
  readonly periods = signal<BalancePeriod[]>([]);
  readonly summary = signal<AnnualSummary | null>(null);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly availableYears = computed(() =>
    [...new Set(this.periods().map(p => p.year))].sort((a, b) => b - a));

  selectedYear: number | null = null;

  constructor(private readonly balanceService: BalanceService) {}

  ngOnInit(): void {
    this.balanceService.getPeriods().subscribe({
      next: p => {
        this.periods.set(p);
        if (p.length > 0) {
          this.selectedYear = p[0].year;
          this.loadSummary();
        }
      },
      error: err => this.errorMessage.set(extractProblemMessage(err, 'Failed to load periods'))
    });
  }

  onYearChange(): void {
    this.loadSummary();
  }

  formatAmount(amount: number | null): string {
    return formatRupee(amount);
  }

  isNegative(amount: number | null): boolean {
    return isNegativeAmount(amount);
  }

  private loadSummary(): void {
    if (this.selectedYear === null) return;
    this.loading.set(true);
    this.errorMessage.set(null);

    this.balanceService.getAnnualSummary(this.selectedYear).subscribe({
      next: data => { this.summary.set(data); this.loading.set(false); },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set(extractProblemMessage(err, 'Failed to load annual summary'));
      }
    });
  }
}
