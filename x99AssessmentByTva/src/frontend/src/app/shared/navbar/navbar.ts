import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.html'
})
export class Navbar {
  constructor(
    public readonly auth: AuthService,
    public readonly theme: ThemeService,
    private readonly router: Router) {}

  onToggleTheme(): void {
    this.theme.toggle();
  }

  onLogout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
