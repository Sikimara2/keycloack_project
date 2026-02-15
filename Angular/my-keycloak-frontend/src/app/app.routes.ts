import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

/**
 * Application Routes with Role-Based Guards.
 *
 * HOW ROUTE PROTECTION WORKS:
 * 1. Public routes (/, /login) - No guard, anyone can access
 * 2. Protected routes (/dashboard) - authGuard checks if user is authenticated
 * 3. Role-specific routes - authGuard also checks if user has the required role
 *
 * The guard reads the 'role' from route data and checks it against Keycloak roles.
 */
export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'custom-login',
    loadComponent: () =>
      import('./pages/login/custom-login.component').then((m) => m.CustomLoginComponent),
  },
  {
    path: 'register',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/register/register.component').then(
        (m) => m.RegisterComponent
      ),
  },
  {
    path: 'custom-register',
    loadComponent: () =>
      import('./pages/register/custom-register.component').then(
        (m) => m.CustomRegisterComponent
      ),
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/dashboard/dashboard.component').then(
        (m) => m.DashboardComponent
      ),
  },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./pages/unauthorized/unauthorized.component').then(
        (m) => m.UnauthorizedComponent
      ),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
