import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { KeycloakService } from '../../services/keycloak.service';

/**
 * Custom Login Component - Uses backend API for authentication
 * instead of Keycloak's hosted login page
 */
@Component({
  selector: 'app-custom-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <h1>Welcome Back</h1>
          <p>Sign in to your account</p>
        </div>

        <form (ngSubmit)="onLogin()" #loginForm="ngForm">
          <div class="form-group">
            <label for="email">Email Address</label>
            <input
              id="email"
              type="email"
              [(ngModel)]="credentials.email"
              name="email"
              required
              email
              placeholder="your.email@example.com"
              [disabled]="isLoading()"
            />
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <input
              id="password"
              type="password"
              [(ngModel)]="credentials.password"
              name="password"
              required
              placeholder="Enter your password"
              [disabled]="isLoading()"
            />
          </div>

          @if (errorMessage()) {
            <div class="error-message">
              {{ errorMessage() }}
            </div>
          }

          <button
            type="submit"
            class="btn-login"
            [disabled]="!loginForm.form.valid || isLoading()"
          >
            @if (isLoading()) {
              <span class="spinner"></span>
              Signing in...
            } @else {
              Sign In
            }
          </button>
        </form>

        <div class="divider">
          <span>OR</span>
        </div>

        <div class="register-section">
          <p>Don't have an account?</p>
          <a routerLink="/custom-register" class="btn-register">
            Create New Account
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 20px;
    }

    .login-card {
      background: white;
      border-radius: 16px;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
      padding: 40px;
      max-width: 420px;
      width: 100%;
    }

    .login-header {
      text-align: center;
      margin-bottom: 32px;
    }

    .login-header h1 {
      margin: 0 0 8px 0;
      color: #2d3748;
      font-size: 28px;
      font-weight: 700;
    }

    .login-header p {
      margin: 0;
      color: #718096;
      font-size: 14px;
    }

    .form-group {
      margin-bottom: 20px;
    }

    .form-group label {
      display: block;
      margin-bottom: 8px;
      font-weight: 600;
      color: #4a5568;
      font-size: 14px;
    }

    .form-group input {
      width: 100%;
      padding: 12px 16px;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 15px;
      box-sizing: border-box;
      transition: border-color 0.3s;
    }

    .form-group input:focus {
      outline: none;
      border-color: #667eea;
    }

    .form-group input:disabled {
      background-color: #f7fafc;
      cursor: not-allowed;
    }

    .error-message {
      background: #fed7d7;
      color: #9b2c2c;
      padding: 12px;
      border-radius: 8px;
      margin-bottom: 20px;
      font-size: 14px;
      text-align: center;
    }

    .btn-login {
      width: 100%;
      padding: 14px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border: none;
      border-radius: 8px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: transform 0.2s, box-shadow 0.2s;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
    }

    .btn-login:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
    }

    .btn-login:disabled {
      opacity: 0.6;
      cursor: not-allowed;
      transform: none;
    }

    .spinner {
      width: 16px;
      height: 16px;
      border: 2px solid #ffffff;
      border-top-color: transparent;
      border-radius: 50%;
      animation: spin 0.6s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .divider {
      text-align: center;
      margin: 24px 0;
      position: relative;
    }

    .divider::before {
      content: '';
      position: absolute;
      top: 50%;
      left: 0;
      right: 0;
      height: 1px;
      background: #e2e8f0;
    }

    .divider span {
      position: relative;
      background: white;
      padding: 0 16px;
      color: #a0aec0;
      font-size: 12px;
      font-weight: 600;
    }

    .register-section {
      text-align: center;
    }

    .register-section p {
      margin: 0 0 12px 0;
      color: #718096;
      font-size: 14px;
    }

    .btn-register {
      display: inline-block;
      padding: 10px 24px;
      background: white;
      color: #667eea;
      border: 2px solid #667eea;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 600;
      text-decoration: none;
      transition: all 0.3s;
    }

    .btn-register:hover {
      background: #667eea;
      color: white;
    }
  `],
})
export class CustomLoginComponent {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);
  private readonly keycloak = inject(KeycloakService);

  credentials = {
    email: '',
    password: '',
  };

  isLoading = signal(false);
  errorMessage = signal('');

  async onLogin() {
    this.isLoading.set(true);
    this.errorMessage.set('');

    try {
      const response = await this.api.login(this.credentials).toPromise();

      if (response) {
        // Store tokens and user info in KeycloakService
        await this.keycloak.setCustomAuthTokens(
          response.accessToken,
          response.refreshToken,
          response.userInfo
        );

        // Navigate to dashboard
        this.router.navigate(['/dashboard']);
      }
    } catch (error: any) {
      this.errorMessage.set(
        error.error?.message || 'Invalid email or password. Please try again.'
      );
    } finally {
      this.isLoading.set(false);
    }
  }
}
