import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap } from 'rxjs';
import { KeycloakService } from '../services/keycloak.service';

/**
 * Auth Interceptor - Automatically attaches the Keycloak JWT token to API requests.
 *
 * HOW HTTP INTERCEPTORS WORK:
 * - Interceptors sit between your app and the HTTP client
 * - Every HTTP request passes through all registered interceptors
 * - We use this to automatically add the Authorization header
 *
 * Without this, you'd have to manually add the token to every API call:
 *   http.get('/api/admin/dashboard', { headers: { Authorization: 'Bearer ...' } })
 *
 * With the interceptor, the token is added automatically.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const keycloakService = inject(KeycloakService);

  // Only add token to API requests (not to external URLs)
  if (!req.url.includes('/api/')) {
    return next(req);
  }

  // Get the token (may need to refresh it first)
  return from(keycloakService.getToken()).pipe(
    switchMap((token) => {
      if (token) {
        // Clone the request and add the Authorization header
        const authReq = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`,
          },
        });
        return next(authReq);
      }
      return next(req);
    })
  );
};
