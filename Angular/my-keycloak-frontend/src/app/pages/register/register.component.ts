import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { KeycloakService } from '../../services/keycloak.service';
import { ApiService } from '../../services/api.service';

/**
 * Registration Component - Collects role-specific profile data.
 *
 * FLOW:
 * 1. User registers with Keycloak (creates account with name/email/password)
 * 2. Admin assigns a role to the user in Keycloak
 * 3. User logs in and is redirected here to fill in role-specific fields
 * 4. This form sends the extra fields to our backend API
 *
 * Common fields (firstName, lastName, email) are already in Keycloak.
 * This form only collects the EXTRA fields specific to each role.
 */
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="register-container">
      <div class="register-card">
        <h2>Complete Your Profile</h2>
        <p class="subtitle">
          Welcome {{ keycloak.firstName() }}! Your role is:
          <strong>{{ keycloak.primaryRole() }}</strong>
        </p>

        <!-- Admin Registration Form -->
        @if (keycloak.primaryRole() === 'admin') {
          <form (ngSubmit)="registerAdmin()">
            <div class="form-group">
              <label for="department">Department</label>
              <input id="department" type="text" [(ngModel)]="adminForm.department"
                     name="department" required placeholder="e.g., IT, Operations" />
            </div>
            <div class="form-group">
              <label for="employeeId">Employee ID</label>
              <input id="employeeId" type="text" [(ngModel)]="adminForm.employeeId"
                     name="employeeId" required placeholder="e.g., EMP001" />
            </div>
            <button type="submit" class="btn btn-primary">Complete Registration</button>
          </form>
        }

        <!-- Gerant Registration Form -->
        @if (keycloak.primaryRole() === 'gerant') {
          <form (ngSubmit)="registerGerant()">
            <div class="form-group">
              <label for="zone">Zone</label>
              <input id="zone" type="text" [(ngModel)]="gerantForm.zone"
                     name="zone" required placeholder="e.g., North, South, East" />
            </div>
            <div class="form-group">
              <label for="phone">Phone Number</label>
              <input id="phone" type="tel" [(ngModel)]="gerantForm.phoneNumber"
                     name="phoneNumber" required placeholder="+33 6 12 34 56 78" />
            </div>
            <button type="submit" class="btn btn-primary">Complete Registration</button>
          </form>
        }

        <!-- Transporteur Registration Form -->
        @if (keycloak.primaryRole() === 'transporteur') {
          <form (ngSubmit)="registerTransporteur()">
            <div class="form-group">
              <label for="license">License Number</label>
              <input id="license" type="text" [(ngModel)]="transporteurForm.licenseNumber"
                     name="licenseNumber" required placeholder="DL-123456" />
            </div>
            <div class="form-group">
              <label for="vehicleType">Vehicle Type</label>
              <select id="vehicleType" [(ngModel)]="transporteurForm.vehicleType"
                      name="vehicleType" required>
                <option value="">Select vehicle type</option>
                <option value="Van">Van</option>
                <option value="Truck">Truck</option>
                <option value="Motorcycle">Motorcycle</option>
                <option value="Car">Car</option>
              </select>
            </div>
            <div class="form-group">
              <label for="plate">Vehicle Plate</label>
              <input id="plate" type="text" [(ngModel)]="transporteurForm.vehiclePlate"
                     name="vehiclePlate" required placeholder="AB-123-CD" />
            </div>
            <div class="form-group">
              <label for="tPhone">Phone Number</label>
              <input id="tPhone" type="tel" [(ngModel)]="transporteurForm.phoneNumber"
                     name="phoneNumber" required placeholder="+33 6 12 34 56 78" />
            </div>
            <button type="submit" class="btn btn-primary">Complete Registration</button>
          </form>
        }

        @if (message()) {
          <div class="message" [class.error]="isError()">{{ message() }}</div>
        }
      </div>
    </div>
  `,
  styles: [`
    .register-container {
      display: flex;
      justify-content: center;
      padding: 40px 20px;
      min-height: 80vh;
    }
    .register-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.1);
      padding: 30px;
      max-width: 500px;
      width: 100%;
    }
    h2 { margin: 0 0 8px 0; color: #2d3748; }
    .subtitle { color: #718096; margin: 0 0 24px 0; }
    .form-group { margin-bottom: 16px; }
    .form-group label {
      display: block;
      margin-bottom: 6px;
      font-weight: 600;
      color: #4a5568;
      font-size: 14px;
    }
    .form-group input, .form-group select {
      width: 100%;
      padding: 10px 12px;
      border: 1px solid #e2e8f0;
      border-radius: 6px;
      font-size: 14px;
      box-sizing: border-box;
    }
    .form-group input:focus, .form-group select:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.15);
    }
    .btn-primary {
      width: 100%;
      padding: 12px;
      background: #667eea;
      color: white;
      border: none;
      border-radius: 8px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      margin-top: 8px;
    }
    .btn-primary:hover { background: #5a6fd6; }
    .message {
      margin-top: 16px;
      padding: 12px;
      border-radius: 6px;
      background: #c6f6d5;
      color: #276749;
      text-align: center;
    }
    .message.error {
      background: #fed7d7;
      color: #9b2c2c;
    }
  `],
})
export class RegisterComponent {
  readonly keycloak = inject(KeycloakService);
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  message = signal('');
  isError = signal(false);

  adminForm = { department: '', employeeId: '' };
  gerantForm = { zone: '', phoneNumber: '' };
  transporteurForm = {
    licenseNumber: '',
    vehicleType: '',
    vehiclePlate: '',
    phoneNumber: '',
  };

  registerAdmin(): void {
    this.api.registerAdmin(this.adminForm).subscribe({
      next: () => {
        this.message.set('Registration complete!');
        this.isError.set(false);
        setTimeout(() => this.router.navigate(['/dashboard']), 1500);
      },
      error: (err) => {
        this.message.set(err.error?.message ?? 'Registration failed');
        this.isError.set(true);
      },
    });
  }

  registerGerant(): void {
    this.api.registerGerant(this.gerantForm).subscribe({
      next: () => {
        this.message.set('Registration complete!');
        this.isError.set(false);
        setTimeout(() => this.router.navigate(['/dashboard']), 1500);
      },
      error: (err) => {
        this.message.set(err.error?.message ?? 'Registration failed');
        this.isError.set(true);
      },
    });
  }

  registerTransporteur(): void {
    this.api.registerTransporteur(this.transporteurForm).subscribe({
      next: () => {
        this.message.set('Registration complete!');
        this.isError.set(false);
        setTimeout(() => this.router.navigate(['/dashboard']), 1500);
      },
      error: (err) => {
        this.message.set(err.error?.message ?? 'Registration failed');
        this.isError.set(true);
      },
    });
  }
}
