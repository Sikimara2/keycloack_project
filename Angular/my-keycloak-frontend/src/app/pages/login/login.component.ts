import { Component, inject } from '@angular/core';
import { KeycloakService } from '../../services/keycloak.service';

/**
 * Custom Login Page.
 *
 * NOTE: Even though we have a custom login page, the actual authentication
 * is handled by Keycloak. When the user clicks "Login", we redirect them
 * to Keycloak's login endpoint. Keycloak handles:
 * - Username/password validation
 * - Session management
 * - Token generation
 *
 * After successful login, Keycloak redirects back to our app with tokens.
 *
 * If you want a FULLY custom login form (no redirect to Keycloak),
 * you'd need to use Keycloak's Direct Access Grant (Resource Owner Password)
 * flow, which sends credentials directly via API. This is less secure and
 * not recommended for browser apps.
 */
@Component({
  selector: 'app-login',
  standalone: true,
  template: `
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <h1>Transport Management</h1>
          <p>Sign in to access your dashboard</p>
        </div>

        <div class="login-body">
          <p class="info-text">
            Authentication is handled securely by Keycloak.
            Click below to sign in with your account.
          </p>

          <button class="btn btn-primary" (click)="login()">
            Sign In with Keycloak
          </button>

          <div class="divider">
            <span>or</span>
          </div>

          <button class="btn btn-secondary" (click)="register()">
            Create New Account
          </button>
        </div>

        <div class="login-footer">
          <h3>Available Roles</h3>
          <div class="roles-info">
            <div class="role-card">
              <strong>Admin</strong>
              <p>Full system access, manage users and assignments</p>
            </div>
            <div class="role-card">
              <strong>Gerant (Manager)</strong>
              <p>Manage transporteurs in your zone</p>
            </div>
            <div class="role-card">
              <strong>Transporteur (Driver)</strong>
              <p>View assignments and update availability</p>
            </div>
          </div>
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
      border-radius: 12px;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
      max-width: 500px;
      width: 100%;
      overflow: hidden;
    }
    .login-header {
      background: #2d3748;
      color: white;
      padding: 30px;
      text-align: center;
    }
    .login-header h1 { margin: 0 0 8px 0; font-size: 24px; }
    .login-header p { margin: 0; opacity: 0.8; }
    .login-body {
      padding: 30px;
      display: flex;
      flex-direction: column;
      gap: 16px;
    }
    .info-text { color: #718096; text-align: center; margin: 0; }
    .btn {
      padding: 12px 24px;
      border: none;
      border-radius: 8px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: transform 0.1s;
    }
    .btn:active { transform: scale(0.98); }
    .btn-primary {
      background: #667eea;
      color: white;
    }
    .btn-primary:hover { background: #5a6fd6; }
    .btn-secondary {
      background: #edf2f7;
      color: #2d3748;
    }
    .btn-secondary:hover { background: #e2e8f0; }
    .divider {
      display: flex;
      align-items: center;
      gap: 16px;
      color: #a0aec0;
    }
    .divider::before, .divider::after {
      content: '';
      flex: 1;
      height: 1px;
      background: #e2e8f0;
    }
    .login-footer {
      padding: 20px 30px 30px;
      background: #f7fafc;
      border-top: 1px solid #e2e8f0;
    }
    .login-footer h3 { margin: 0 0 12px 0; color: #2d3748; font-size: 16px; }
    .roles-info { display: flex; flex-direction: column; gap: 8px; }
    .role-card {
      padding: 10px 14px;
      background: white;
      border-radius: 6px;
      border: 1px solid #e2e8f0;
    }
    .role-card strong { color: #2d3748; font-size: 14px; }
    .role-card p { margin: 4px 0 0 0; color: #718096; font-size: 13px; }
  `],
})
export class LoginComponent {
  private readonly keycloak = inject(KeycloakService);

  login(): void {
    this.keycloak.login();
  }

  register(): void {
    this.keycloak.register();
  }
}
