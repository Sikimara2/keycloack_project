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
        <div class="login-options">
          <h3>Choose your login method:</h3>
          <button class="btn btn-primary" (click)="goToCustomLogin()">
            Custom Login Form
          </button>
          <button class="btn btn-secondary" (click)="goToLogin()">
            Keycloak Hosted Login
          </button>
          <p class="register-hint">
            Don't have an account? <a (click)="goToCustomRegister()">Create one here</a>
          </p>
        </div>
      }
    </div>
  `,
  styles: [`
    .home { text-align: center; padding: 80px 20px; max-width: 600px; margin: 0 auto; }
    h1 { color: #2d3748; font-size: 32px; margin-bottom: 12px; }
    p { color: #718096; font-size: 18px; }
    .authenticated-info { margin-top: 24px; }
    .login-options { margin-top: 32px; }
    .login-options h3 { color: #4a5568; font-size: 18px; margin-bottom: 20px; }
    .btn {
      display: block;
      width: 100%;
      max-width: 300px;
      margin: 12px auto;
      padding: 14px 32px;
      border: none;
      border-radius: 8px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s;
    }
    .btn-primary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }
    .btn-primary:hover { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4); }
    .btn-secondary {
      background: white;
      color: #667eea;
      border: 2px solid #667eea;
    }
    .btn-secondary:hover { background: #f7fafc; }
    .register-hint { margin-top: 24px; font-size: 14px; color: #718096; }
    .register-hint a { color: #667eea; cursor: pointer; text-decoration: underline; }
  `],
})
export class HomeComponent {
  readonly keycloak = inject(KeycloakService);
  private readonly router = inject(Router);

  goToLogin(): void { this.router.navigate(['/login']); }
  goToCustomLogin(): void { this.router.navigate(['/custom-login']); }
  goToCustomRegister(): void { this.router.navigate(['/custom-register']); }
  goToDashboard(): void { this.router.navigate(['/dashboard']); }
}
