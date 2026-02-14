import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { KeycloakService } from '../../services/keycloak.service';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { GerantDashboardComponent } from './gerant-dashboard/gerant-dashboard.component';
import { TransporteurDashboardComponent } from './transporteur-dashboard/transporteur-dashboard.component';

/**
 * Dashboard Router Component.
 * Shows the correct dashboard based on the user's Keycloak role.
 */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, AdminDashboardComponent, GerantDashboardComponent, TransporteurDashboardComponent],
  template: `
    @switch (keycloak.primaryRole()) {
      @case ('admin') {
        <app-admin-dashboard />
      }
      @case ('gerant') {
        <app-gerant-dashboard />
      }
      @case ('transporteur') {
        <app-transporteur-dashboard />
      }
      @default {
        <div class="no-role">
          <h2>No Role Assigned</h2>
          <p>Your account doesn't have a role yet. Please contact an administrator.</p>
          <p>Your current roles: {{ keycloak.userRoles().join(', ') || 'none' }}</p>
        </div>
      }
    }
  `,
  styles: [`
    .no-role {
      text-align: center;
      padding: 60px 20px;
      color: #718096;
    }
    .no-role h2 { color: #2d3748; }
  `],
})
export class DashboardComponent {
  readonly keycloak = inject(KeycloakService);
}
