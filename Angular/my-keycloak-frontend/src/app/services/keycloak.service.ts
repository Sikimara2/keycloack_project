import { Injectable, signal, computed } from '@angular/core';
import Keycloak from 'keycloak-js';
import { UserRole } from '../models/user.model';

/**
 * KeycloakService - Manages authentication with Keycloak.
 *
 * HOW IT WORKS:
 * 1. On app startup, we initialize the Keycloak JS adapter
 * 2. The adapter handles the OpenID Connect flow (redirects to Keycloak login page)
 * 3. After login, Keycloak issues tokens (access token + refresh token)
 * 4. The access token is a JWT containing user info and roles
 * 5. We extract roles from the token to control UI access
 * 6. The token is sent with every API request (via HTTP interceptor)
 *
 * KEYCLOAK CONFIGURATION:
 * - url: Where Keycloak server runs (e.g., http://localhost:8080)
 * - realm: The Keycloak realm (like a tenant/organization)
 * - clientId: The application registered in Keycloak
 */
@Injectable({ providedIn: 'root' })
export class KeycloakService {
  private keycloak: Keycloak | null = null;

  // Reactive signals for auth state
  private readonly _isAuthenticated = signal(false);
  private readonly _userRoles = signal<string[]>([]);
  private readonly _userName = signal('');
  private readonly _userEmail = signal('');
  private readonly _firstName = signal('');
  private readonly _lastName = signal('');

  // Public readonly signals
  readonly isAuthenticated = this._isAuthenticated.asReadonly();
  readonly userRoles = this._userRoles.asReadonly();
  readonly userName = this._userName.asReadonly();
  readonly userEmail = this._userEmail.asReadonly();
  readonly firstName = this._firstName.asReadonly();
  readonly lastName = this._lastName.asReadonly();

  // Computed: determine the primary role
  readonly primaryRole = computed<UserRole | null>(() => {
    const roles = this._userRoles();
    if (roles.includes('admin')) return 'admin';
    if (roles.includes('gerant')) return 'gerant';
    if (roles.includes('transporteur')) return 'transporteur';
    return null;
  });

  /**
   * Initialize Keycloak - called once on app startup.
   * Uses 'check-sso' to silently check if user is already logged in.
   */
  async init(): Promise<boolean> {
    this.keycloak = new Keycloak({
      url: 'http://localhost:8080',
      realm: 'transport-realm',
      clientId: 'transport-app',
    });

    try {
      const authenticated = await this.keycloak.init({
        onLoad: 'check-sso',          // Don't force login, just check
        silentCheckSsoRedirectUri:
          window.location.origin + '/assets/silent-check-sso.html',
        checkLoginIframe: false,       // Disable iframe check (can cause issues)
      });

      this._isAuthenticated.set(authenticated);

      if (authenticated) {
        this.loadUserProfile();
      }

      // Set up automatic token refresh
      this.keycloak.onTokenExpired = () => {
        this.keycloak?.updateToken(30).catch(() => {
          this.logout();
        });
      };

      return authenticated;
    } catch (error) {
      console.error('Keycloak init failed:', error);
      this._isAuthenticated.set(false);
      return false;
    }
  }

  /**
   * Redirect to Keycloak login page.
   * After login, Keycloak redirects back to our app with tokens.
   */
  login(): void {
    this.keycloak?.login({
      redirectUri: window.location.origin + '/dashboard',
    });
  }

  /**
   * Register a new user via Keycloak's registration page.
   * Keycloak handles user creation, password hashing, etc.
   */
  register(): void {
    this.keycloak?.register({
      redirectUri: window.location.origin + '/dashboard',
    });
  }

  /**
   * Logout - clears tokens and redirects to Keycloak logout endpoint.
   */
  logout(): void {
    this.keycloak?.logout({
      redirectUri: window.location.origin,
    });
  }

  /**
   * Get the current access token for API calls.
   * Returns a promise because the token might need to be refreshed first.
   */
  async getToken(): Promise<string> {
    if (!this.keycloak?.authenticated) {
      return '';
    }

    try {
      // Refresh token if it expires within 30 seconds
      await this.keycloak.updateToken(30);
      return this.keycloak.token ?? '';
    } catch {
      this.logout();
      return '';
    }
  }

  /**
   * Check if the user has a specific role.
   */
  hasRole(role: string): boolean {
    return this._userRoles().includes(role);
  }

  /**
   * Extract user profile info from the Keycloak token.
   */
  private loadUserProfile(): void {
    if (!this.keycloak?.tokenParsed) return;

    const token = this.keycloak.tokenParsed;

    this._userName.set(token['preferred_username'] ?? '');
    this._userEmail.set(token['email'] ?? '');
    this._firstName.set(token['given_name'] ?? '');
    this._lastName.set(token['family_name'] ?? '');

    // Extract realm roles
    const realmRoles = token['realm_access']?.['roles'] ?? [];
    // Extract client roles
    const clientRoles =
      token['resource_access']?.['transport-app']?.['roles'] ?? [];

    // Combine and deduplicate
    const allRoles = [...new Set([...realmRoles, ...clientRoles])];
    this._userRoles.set(allRoles);
  }
}
