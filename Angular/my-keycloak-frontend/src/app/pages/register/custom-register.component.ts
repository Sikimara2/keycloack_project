import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';

type UserRole = 'admin' | 'gerant' | 'transporteur';

/**
 * Custom Registration Component - Comprehensive registration with many fields
 * Creates user in Keycloak and saves profile data in backend
 */
@Component({
  selector: 'app-custom-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './custom-register.component.html',
  styleUrls: ['./custom-register.component.css'],
})
export class CustomRegisterComponent {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  selectedRole = signal<UserRole | null>(null);
  isLoading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  // Common fields for all roles
  commonForm = {
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
  };

  // Admin-specific fields
  adminForm = {
    department: '',
    employeeId: '',
    phoneNumber: '',
    jobTitle: '',
    officeLocation: '',
    directLine: '',
    hireDate: '',
    managerName: '',
    canManageUsers: true,
    canViewReports: true,
    canManageSettings: true,
    canApproveExpenses: false,
  };

  // Gerant-specific fields
  gerantForm = {
    zone: '',
    phoneNumber: '',
    mobileNumber: '',
    maxTransporteursManaged: 10,
    address: '',
    city: '',
    postalCode: '',
    emergencyContact: '',
    emergencyPhone: '',
    dateOfBirth: '',
    nationality: '',
    languagesSpoken: '',
    yearsOfExperience: 0,
    previousEmployer: '',
  };

  // Transporteur-specific fields
  transporteurForm = {
    licenseNumber: '',
    vehicleType: '',
    vehiclePlate: '',
    phoneNumber: '',
    mobileNumber: '',
    address: '',
    city: '',
    postalCode: '',
    licenseExpiryDate: '',
    dateOfBirth: '',
    nationality: '',
    emergencyContact: '',
    emergencyPhone: '',
    vehicleMake: '',
    vehicleModel: '',
    vehicleYear: new Date().getFullYear(),
    vehicleColor: '',
    insuranceProvider: '',
    insurancePolicyNumber: '',
    insuranceExpiryDate: '',
    yearsOfExperience: 0,
    languagesSpoken: '',
    hasCommercialLicense: false,
    hasHazmatCertification: false,
  };

  selectRole(role: UserRole) {
    this.selectedRole.set(role);
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  backToRoleSelection() {
    this.selectedRole.set(null);
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  validateCommonFields(): boolean {
    if (!this.commonForm.email.includes('@')) {
      this.errorMessage.set('Please enter a valid email address');
      return false;
    }

    if (this.commonForm.password.length < 6) {
      this.errorMessage.set('Password must be at least 6 characters long');
      return false;
    }

    if (this.commonForm.password !== this.commonForm.confirmPassword) {
      this.errorMessage.set('Passwords do not match');
      return false;
    }

    if (!this.commonForm.firstName || !this.commonForm.lastName) {
      this.errorMessage.set('First name and last name are required');
      return false;
    }

    return true;
  }

  async onSubmit() {
    if (!this.validateCommonFields()) {
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    try {
      const role = this.selectedRole();

      if (role === 'admin') {
        await this.registerAdmin();
      } else if (role === 'gerant') {
        await this.registerGerant();
      } else if (role === 'transporteur') {
        await this.registerTransporteur();
      }

      this.successMessage.set('Registration successful! Redirecting to login...');
      setTimeout(() => {
        this.router.navigate(['/custom-login']);
      }, 2000);
    } catch (error: any) {
      this.errorMessage.set(
        error.error?.message || 'Registration failed. Please try again.'
      );
    } finally {
      this.isLoading.set(false);
    }
  }

  private async registerAdmin() {
    const payload = {
      ...this.commonForm,
      ...this.adminForm,
      hireDate: this.adminForm.hireDate || null,
    };

    await this.api.registerAdminFull(payload).toPromise();
  }

  private async registerGerant() {
    const payload = {
      ...this.commonForm,
      ...this.gerantForm,
      dateOfBirth: this.gerantForm.dateOfBirth || null,
    };

    await this.api.registerGerantFull(payload).toPromise();
  }

  private async registerTransporteur() {
    const payload = {
      ...this.commonForm,
      ...this.transporteurForm,
      licenseExpiryDate: this.transporteurForm.licenseExpiryDate || null,
      dateOfBirth: this.transporteurForm.dateOfBirth || null,
      insuranceExpiryDate: this.transporteurForm.insuranceExpiryDate || null,
      vehicleYear: this.transporteurForm.vehicleYear || null,
    };

    await this.api.registerTransporteurFull(payload).toPromise();
  }
}
