import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakService } from '../../services/keycloak.service';

@Component({
  selector: 'app-home',
  standalone: true,
  template: `
    <div class="home">
      <h1>Transport Management System</h1>
      <p>A demo application showing Keycloak integration with Angular and ASP.NET</p>

      @if (keycloak.isAuthenticated()) {
        <div class="authenticated-info">
          <p>You are logged in as <strong>{{ keycloak.userName() }}</strong></p>
          <p>Roles: {{ keycloak.userRoles().join(', ') }}</p>
          <button class="btn" (click)="goToDashboard()">Go to Dashboard</button>
        </div>
      } @else {
        <button class="btn" (click)="goToLogin()">Get Started</button>
      }
    </div>
  `,
  styles: [`
    .home { text-align: center; padding: 80px 20px; max-width: 600px; margin: 0 auto; }
    h1 { color: #2d3748; font-size: 32px; margin-bottom: 12px; }
    p { color: #718096; font-size: 18px; }
    .authenticated-info { margin-top: 24px; }
    .btn {
      margin-top: 16px; padding: 12px 32px; background: #667eea; color: white;
      border: none; border-radius: 8px; font-size: 16px; font-weight: 600; cursor: pointer;
    }
    .btn:hover { background: #5a6fd6; }
  `],
})
export class HomeComponent {
  readonly keycloak = inject(KeycloakService);
  private readonly router = inject(Router);

  goToLogin(): void { this.router.navigate(['/login']); }
  goToDashboard(): void { this.router.navigate(['/dashboard']); }
}
