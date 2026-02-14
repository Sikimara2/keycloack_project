import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../services/api.service';
import { KeycloakService } from '../../../services/keycloak.service';
import { GerantDashboard, TransporteurProfile } from '../../../models/user.model';

@Component({
  selector: 'app-gerant-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <h2>Gerant Dashboard</h2>
      <p class="welcome">Welcome, {{ keycloak.firstName() }} {{ keycloak.lastName() }}</p>

      @if (stats()) {
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-number">{{ stats()!.managedTransporteurs }}</div>
            <div class="stat-label">Managed Transporteurs</div>
          </div>
          <div class="stat-card highlight">
            <div class="stat-number">{{ stats()!.availableTransporteurs }}</div>
            <div class="stat-label">Available Now</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ stats()!.zone }}</div>
            <div class="stat-label">Zone</div>
          </div>
        </div>
      }

      <h3>My Transporteurs</h3>
      @if (transporteurs().length > 0) {
        <div class="transporteur-list">
          @for (t of transporteurs(); track t.id) {
            <div class="transporteur-card">
              <div class="t-header">
                <strong>{{ t.firstName }} {{ t.lastName }}</strong>
                <span class="status" [class.available]="t.isAvailable">
                  {{ t.isAvailable ? 'Available' : 'Unavailable' }}
                </span>
              </div>
              <div class="t-details">
                <span>{{ t.vehicleType }} - {{ t.vehiclePlate }}</span>
                <span>{{ t.phoneNumber }}</span>
              </div>
            </div>
          }
        </div>
      } @else {
        <p class="empty">No transporteurs assigned to you yet.</p>
      }
    </div>
  `,
  styles: [`
    .dashboard { padding: 20px; max-width: 800px; margin: 0 auto; }
    h2 { color: #2d3748; margin: 0; }
    .welcome { color: #718096; margin: 4px 0 24px; }
    .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px; margin-bottom: 32px; }
    .stat-card { background: white; border-radius: 8px; padding: 20px; text-align: center; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .stat-card.highlight { background: #48bb78; color: white; }
    .stat-number { font-size: 28px; font-weight: 700; }
    .stat-label { font-size: 14px; opacity: 0.8; margin-top: 4px; }
    h3 { color: #2d3748; margin: 0 0 16px; }
    .transporteur-list { display: flex; flex-direction: column; gap: 12px; }
    .transporteur-card { background: white; border-radius: 8px; padding: 16px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .t-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px; }
    .status { padding: 4px 10px; border-radius: 12px; font-size: 12px; font-weight: 600; background: #fed7d7; color: #9b2c2c; }
    .status.available { background: #c6f6d5; color: #276749; }
    .t-details { display: flex; justify-content: space-between; color: #718096; font-size: 14px; }
    .empty { color: #a0aec0; text-align: center; padding: 40px; }
  `],
})
export class GerantDashboardComponent implements OnInit {
  readonly keycloak = inject(KeycloakService);
  private readonly api = inject(ApiService);

  stats = signal<GerantDashboard | null>(null);
  transporteurs = signal<TransporteurProfile[]>([]);

  ngOnInit(): void {
    this.api.getGerantDashboard().subscribe({
      next: (data) => this.stats.set(data),
      error: (err) => console.error('Failed to load gerant dashboard:', err),
    });

    this.api.getMyTransporteurs().subscribe({
      next: (data) => this.transporteurs.set(data),
      error: (err) => console.error('Failed to load transporteurs:', err),
    });
  }
}
