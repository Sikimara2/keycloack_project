import {
  ApplicationConfig,
  APP_INITIALIZER,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { KeycloakService } from './services/keycloak.service';
import { authInterceptor } from './interceptors/auth.interceptor';

/**
 * Application Configuration.
 *
 * KEY CONCEPTS:
 *
 * 1. APP_INITIALIZER: Runs Keycloak init BEFORE the app starts.
 *    This ensures we know the user's auth state before any component renders.
 *
 * 2. provideHttpClient + withInterceptors: Registers our auth interceptor
 *    that automatically adds the JWT token to API requests.
 *
 * 3. provideRouter: Sets up client-side routing with lazy-loaded components.
 */

function initializeKeycloak(keycloak: KeycloakService): () => Promise<boolean> {
  return () => keycloak.init();
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeKeycloak,
      deps: [KeycloakService],
      multi: true,
    },
  ],
};
