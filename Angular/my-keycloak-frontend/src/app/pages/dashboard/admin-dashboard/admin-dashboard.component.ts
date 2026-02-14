import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../services/api.service';
import { KeycloakService } from '../../../services/keycloak.service';
import { AdminDashboard, UserProfile } from '../../../models/user.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <h2>Admin Dashboard</h2>
      <p class="welcome">Welcome, {{ keycloak.firstName() }} {{ keycloak.lastName() }}</p>

      @if (stats()) {
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-number">{{ stats()!.totalUsers }}</div>
            <div class="stat-label">Total Users</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ stats()!.totalAdmins }}</div>
            <div class="stat-label">Admins</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ stats()!.totalGerants }}</div>
            <div class="stat-label">Gerants</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ stats()!.totalTransporteurs }}</div>
            <div class="stat-label">Transporteurs</div>
          </div>
          <div class="stat-card highlight">
            <div class="stat-number">{{ stats()!.activeTransporteurs }}</div>
            <div class="stat-label">Active Transporteurs</div>
          </div>
        </div>
      }

      <h3>All Users</h3>
      @if (users().length > 0) {
        <table class="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Role</th>
              <th>Created</th>
            </tr>
          </thead>
          <tbody>
            @for (user of users(); track user.id) {
              <tr>
                <td>{{ user.firstName }} {{ user.lastName }}</td>
                <td>{{ user.email }}</td>
                <td><span class="role-badge" [attr.data-role]="user.role">{{ user.role }}</span></td>
                <td>{{ user.createdAt | date:'short' }}</td>
              </tr>
            }
          </tbody>
        </table>
      } @else {
        <p class="empty">No users registered yet. Users need to complete their profile registration.</p>
      }
    </div>
  `,
  styles: [`
    .dashboard { padding: 20px; max-width: 1000px; margin: 0 auto; }
    h2 { color: #2d3748; margin: 0; }
    .welcome { color: #718096; margin: 4px 0 24px; }
    .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(160px, 1fr)); gap: 16px; margin-bottom: 32px; }
    .stat-card { background: white; border-radius: 8px; padding: 20px; text-align: center; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .stat-card.highlight { background: #667eea; color: white; }
    .stat-number { font-size: 32px; font-weight: 700; }
    .stat-label { font-size: 14px; opacity: 0.8; margin-top: 4px; }
    .data-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .data-table th { background: #f7fafc; padding: 12px 16px; text-align: left; font-size: 13px; color: #718096; text-transform: uppercase; }
    .data-table td { padding: 12px 16px; border-top: 1px solid #e2e8f0; }
    .role-badge { padding: 4px 10px; border-radius: 12px; font-size: 12px; font-weight: 600; }
    [data-role="admin"] { background: #fed7d7; color: #9b2c2c; }
    [data-role="gerant"] { background: #c6f6d5; color: #276749; }
    [data-role="transporteur"] { background: #bee3f8; color: #2a4365; }
    .empty { color: #a0aec0; text-align: center; padding: 40px; }
    h3 { color: #2d3748; margin: 0 0 16px; }
  `],
})
export class AdminDashboardComponent implements OnInit {
  readonly keycloak = inject(KeycloakService);
  private readonly api = inject(ApiService);

  stats = signal<AdminDashboard | null>(null);
  users = signal<UserProfile[]>([]);

  ngOnInit(): void {
    this.api.getAdminDashboard().subscribe({
      next: (data) => this.stats.set(data),
      error: (err) => console.error('Failed to load admin dashboard:', err),
    });

    this.api.getAllUsers().subscribe({
      next: (data) => this.users.set(data),
      error: (err) => console.error('Failed to load users:', err),
    });
  }
}
