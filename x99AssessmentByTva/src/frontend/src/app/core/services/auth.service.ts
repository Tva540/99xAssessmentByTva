import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from '../models/auth.models';

const STORAGE_KEY = 'x99-auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _session = signal<LoginResponse | null>(this.loadFromStorage());

  readonly session = this._session.asReadonly();
  readonly isAuthenticated = computed(() => this._session() !== null && !this.isExpired());
  readonly displayName = computed(() => this._session()?.displayName ?? '');
  readonly roles = computed(() => this._session()?.roles ?? []);
  readonly isAdmin = computed(() => this.roles().includes('Admin'));

  constructor(private readonly http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>('/api/auth/login', request).pipe(
      tap(response => {
        this._session.set(response);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
      })
    );
  }

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>('/api/auth/register', request);
  }

  logout(): void {
    this._session.set(null);
    localStorage.removeItem(STORAGE_KEY);
  }

  token(): string | null {
    const session = this._session();
    return session && !this.isExpired() ? session.token : null;
  }

  private isExpired(): boolean {
    const session = this._session();
    if (!session) return true;
    return new Date(session.expiresAt).getTime() <= Date.now();
  }

  private loadFromStorage(): LoginResponse | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    try { return JSON.parse(raw) as LoginResponse; }
    catch { return null; }
  }
}
