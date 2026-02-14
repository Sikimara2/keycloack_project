import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../services/api.service';
import { KeycloakService } from '../../../services/keycloak.service';
import { TransporteurDashboard } from '../../../models/user.model';

@Component({
  selector: 'app-transporteur-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <h2>Transporteur Dashboard</h2>
      <p class="welcome">Welcome, {{ keycloak.firstName() }} {{ keycloak.lastName() }}</p>

      @if (dashboard()) {
        <div class="status-section">
          <div class="availability-card" [class.available]="dashboard()!.isAvailable">
            <div class="availability-icon">{{ dashboard()!.isAvailable ? '&#10003;' : '&#10007;' }}</div>
            <div class="availability-text">
              {{ dashboard()!.isAvailable ? 'You are Available' : 'You are Unavailable' }}
            </div>
            <button class="toggle-btn" (click)="toggleAvailability()">
              {{ dashboard()!.isAvailable ? 'Go Unavailable' : 'Go Available' }}
            </button>
          </div>
        </div>

        <div class="info-grid">
          <div class="info-card">
            <div class="info-label">Vehicle Type</div>
            <div class="info-value">{{ dashboard()!.vehicleType || 'Not set' }}</div>
          </div>
          <div class="info-card">
            <div class="info-label">Assigned Manager</div>
            <div class="info-value">{{ dashboard()!.assignedGerantName || 'None' }}</div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .dashboard { padding: 20px; max-width: 600px; margin: 0 auto; }
    h2 { color: #2d3748; margin: 0; }
    .welcome { color: #718096; margin: 4px 0 24px; }
    .status-section { margin-bottom: 24px; }
    .availability-card {
      background: #fed7d7; border-radius: 12px; padding: 30px;
      text-align: center; transition: background 0.3s;
    }
    .availability-card.available { background: #c6f6d5; }
    .availability-icon { font-size: 48px; margin-bottom: 8px; }
    .availability-text { font-size: 20px; font-weight: 600; color: #2d3748; margin-bottom: 16px; }
    .toggle-btn {
      padding: 10px 24px; border: 2px solid #2d3748; border-radius: 8px;
      background: white; font-size: 14px; font-weight: 600; cursor: pointer;
    }
    .toggle-btn:hover { background: #2d3748; color: white; }
    .info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .info-card { background: white; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .info-label { font-size: 13px; color: #718096; margin-bottom: 4px; }
    .info-value { font-size: 18px; font-weight: 600; color: #2d3748; }
  `],
})
export class TransporteurDashboardComponent implements OnInit {
  readonly keycloak = inject(KeycloakService);
  private readonly api = inject(ApiService);

  dashboard = signal<TransporteurDashboard | null>(null);

  ngOnInit(): void {
    this.loadDashboard();
  }

  toggleAvailability(): void {
    const current = this.dashboard()?.isAvailable ?? false;
    this.api.updateAvailability(!current).subscribe({
      next: () => this.loadDashboard(),
      error: (err) => console.error('Failed to update availability:', err),
    });
  }

  private loadDashboard(): void {
    this.api.getTransporteurDashboard().subscribe({
      next: (data) => this.dashboard.set(data),
      error: (err) => console.error('Failed to load dashboard:', err),
    });
  }
}
