import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { KeycloakService } from '../../services/keycloak.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    <nav class="navbar">
      <a routerLink="/" class="nav-brand">Transport App</a>
      <div class="nav-links">
        @if (keycloak.isAuthenticated()) {
          <a routerLink="/dashboard" routerLinkActive="active">Dashboard</a>
          <span class="user-info">
            {{ keycloak.firstName() }} ({{ keycloak.primaryRole() }})
          </span>
          <button class="logout-btn" (click)="keycloak.logout()">Logout</button>
        } @else {
          <a routerLink="/login" routerLinkActive="active">Login</a>
        }
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      display: flex; justify-content: space-between; align-items: center;
      padding: 12px 24px; background: #2d3748; color: white;
    }
    .nav-brand { color: white; text-decoration: none; font-weight: 700; font-size: 18px; }
    .nav-links { display: flex; align-items: center; gap: 16px; }
    .nav-links a { color: #e2e8f0; text-decoration: none; font-size: 14px; }
    .nav-links a.active { color: white; font-weight: 600; }
    .user-info { color: #a0aec0; font-size: 13px; }
    .logout-btn {
      padding: 6px 14px; background: transparent; border: 1px solid #e2e8f0;
      color: #e2e8f0; border-radius: 6px; font-size: 13px; cursor: pointer;
    }
    .logout-btn:hover { background: rgba(255,255,255,0.1); }
  `],
})
export class NavbarComponent {
  readonly keycloak = inject(KeycloakService);
}
