import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { KeycloakService } from '../services/keycloak.service';

/**
 * Auth Guard - Protects routes that require authentication.
 *
 * HOW ANGULAR GUARDS WORK:
 * - Guards are functions that run BEFORE a route is activated
 * - If the guard returns true, navigation proceeds
 * - If it returns false (or a UrlTree), navigation is blocked/redirected
 *
 * This guard checks:
 * 1. Is the user authenticated? (has a valid Keycloak session)
 * 2. If a specific role is required, does the user have it?
 */
export const authGuard: CanActivateFn = (route) => {
  const keycloakService = inject(KeycloakService);
  const router = inject(Router);

  // Check authentication
  if (!keycloakService.isAuthenticated()) {
    // Redirect to login page
    return router.createUrlTree(['/login']);
  }

  // Check role if specified in route data
  const requiredRole = route.data?.['role'] as string | undefined;
  if (requiredRole && !keycloakService.hasRole(requiredRole)) {
    // User is authenticated but doesn't have the required role
    return router.createUrlTree(['/unauthorized']);
  }

  return true;
};
