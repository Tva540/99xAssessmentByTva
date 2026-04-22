import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { extractProblemMessage } from '../../../core/helpers/problem-details';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.html'
})
export class Login {
  readonly form: FormGroup;
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  constructor(
    fb: FormBuilder,
    private readonly auth: AuthService,
    private readonly router: Router)
  {
    this.form = fb.group({
      email:    ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.errorMessage.set(null);

    this.auth.login(this.form.value).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate([this.auth.isAdmin() ? '/upload' : '/balances']);
      },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set(extractProblemMessage(err, 'Login failed'));
      }
    });
  }
}
