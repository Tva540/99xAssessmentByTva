import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { extractProblemMessage } from '../../../core/helpers/problem-details';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html'
})
export class Register {
  readonly form: FormGroup;
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  constructor(
    fb: FormBuilder,
    private readonly auth: AuthService,
    private readonly router: Router)
  {
    this.form = fb.group({
      displayName: ['', [Validators.required, Validators.maxLength(100)]],
      email:       ['', [Validators.required, Validators.email]],
      password:    ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.auth.register(this.form.value).subscribe({
      next: res => {
        this.loading.set(false);
        this.successMessage.set(
            `Account created for ${res.email}. Redirecting to login...`);
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set(extractProblemMessage(err, 'Registration failed'));
      }
    });
  }
}
