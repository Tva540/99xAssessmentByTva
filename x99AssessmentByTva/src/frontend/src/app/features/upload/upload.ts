import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BalanceService } from '../../core/services/balance.service';
import { UploadResult } from '../../core/models/balance.models';
import { extractProblemMessage } from '../../core/helpers/problem-details';

@Component({
  selector: 'app-upload',
  standalone: false,
  templateUrl: './upload.html'
})
export class Upload {
  readonly form: FormGroup;
  readonly loading = signal(false);
  readonly result = signal<UploadResult | null>(null);
  readonly errorMessage = signal<string | null>(null);

  selectedFile: File | null = null;

  constructor(
    fb: FormBuilder,
    private readonly balanceService: BalanceService)
  {
    const now = new Date();
    this.form = fb.group({
      year: [now.getFullYear(), [Validators.required, Validators.min(1900), Validators.max(2200)]]
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  onSubmit(): void {
    if (!this.selectedFile || this.form.invalid) return;
    this.loading.set(true);
    this.result.set(null);
    this.errorMessage.set(null);

    this.balanceService.upload(this.selectedFile, this.form.value.year).subscribe({
      next: res => { this.result.set(res); this.loading.set(false); },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set(extractProblemMessage(err, 'Upload failed'));
      }
    });
  }
}
