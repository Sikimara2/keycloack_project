// Roles matching the Keycloak realm roles
export type UserRole = 'admin' | 'gerant' | 'transporteur';

// Base profile - common fields from Keycloak
export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  createdAt: string;
}

export interface AdminProfile extends UserProfile {
  department: string;
  employeeId: string;
  canManageUsers: boolean;
  canViewReports: boolean;
  canManageSettings: boolean;
}

export interface GerantProfile extends UserProfile {
  zone: string;
  phoneNumber: string;
  maxTransporteursManaged: number;
  isActive: boolean;
}

export interface TransporteurProfile extends UserProfile {
  licenseNumber: string;
  vehicleType: string;
  vehiclePlate: string;
  phoneNumber: string;
  isAvailable: boolean;
  assignedGerantId?: string;
}

// Dashboard DTOs
export interface AdminDashboard {
  totalUsers: number;
  totalAdmins: number;
  totalGerants: number;
  totalTransporteurs: number;
  activeTransporteurs: number;
}

export interface GerantDashboard {
  managedTransporteurs: number;
  availableTransporteurs: number;
  zone: string;
}

export interface TransporteurDashboard {
  isAvailable: boolean;
  vehicleType: string;
  assignedGerantName: string;
}

// Token info extracted from Keycloak
export interface KeycloakTokenInfo {
  keycloakId: string;
  email: string;
  firstName: string;
  lastName: string;
  preferredUsername: string;
  roles: string[];
}
