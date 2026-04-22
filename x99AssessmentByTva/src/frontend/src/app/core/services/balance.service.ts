    import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Account, AnnualSummary, BalancePeriod, MonthlyBalances, UploadResult } from '../models/balance.models';

@Injectable({ providedIn: 'root' })
export class BalanceService {
  constructor(private readonly http: HttpClient) {}

  getAccounts(): Observable<Account[]> {
    return this.http.get<Account[]>('/api/balances/accounts');
  }

  getPeriods(): Observable<BalancePeriod[]> {
    return this.http.get<BalancePeriod[]>('/api/balances/periods');
  }

  getMonthly(year: number, month: number): Observable<MonthlyBalances> {
    return this.http.get<MonthlyBalances>(`/api/balances/${year}/${month}`);
  }

  upload(file: File, year: number): Observable<UploadResult> {
    const form = new FormData();
    form.append('file', file);
    form.append('year', year.toString());
    return this.http.post<UploadResult>('/api/balances/upload', form);
  }

  getAnnualSummary(year: number): Observable<AnnualSummary> {
    return this.http.get<AnnualSummary>(`/api/balances/annual/${year}`);
  }
}
