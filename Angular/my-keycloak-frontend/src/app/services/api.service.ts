import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AdminDashboard,
  GerantDashboard,
  TransporteurDashboard,
  KeycloakTokenInfo,
  TransporteurProfile,
  GerantProfile,
  UserProfile,
} from '../models/user.model';

/**
 * API Service - Handles all HTTP calls to the ASP.NET backend.
 *
 * The AuthInterceptor automatically attaches the Keycloak JWT token
 * to every request's Authorization header, so we don't need to
 * manually add it here.
 */
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5157/api';

  // --- Profile endpoints (any authenticated user) ---

  getMyProfile(): Observable<KeycloakTokenInfo> {
    return this.http.get<KeycloakTokenInfo>(`${this.baseUrl}/profile/me`);
  }

  registerAdmin(data: { department: string; employeeId: string }): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/profile/register/admin`, data);
  }

  registerGerant(data: { zone: string; phoneNumber: string }): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/profile/register/gerant`, data);
  }

  registerTransporteur(data: {
    licenseNumber: string;
    vehicleType: string;
    vehiclePlate: string;
    phoneNumber: string;
  }): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/profile/register/transporteur`, data);
  }

  // --- Admin endpoints ---

  getAdminDashboard(): Observable<AdminDashboard> {
    return this.http.get<AdminDashboard>(`${this.baseUrl}/admin/dashboard`);
  }

  getAllUsers(): Observable<UserProfile[]> {
    return this.http.get<UserProfile[]>(`${this.baseUrl}/admin/users`);
  }

  getAllGerants(): Observable<GerantProfile[]> {
    return this.http.get<GerantProfile[]>(`${this.baseUrl}/admin/gerants`);
  }

  getAllTransporteurs(): Observable<TransporteurProfile[]> {
    return this.http.get<TransporteurProfile[]>(`${this.baseUrl}/admin/transporteurs`);
  }

  assignTransporteur(transporteurId: string, gerantId: string): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/admin/assign-transporteur`, {
      transporteurId,
      gerantId,
    });
  }

  // --- Gerant endpoints ---

  getGerantDashboard(): Observable<GerantDashboard> {
    return this.http.get<GerantDashboard>(`${this.baseUrl}/gerant/dashboard`);
  }

  getMyTransporteurs(): Observable<TransporteurProfile[]> {
    return this.http.get<TransporteurProfile[]>(`${this.baseUrl}/gerant/my-transporteurs`);
  }

  getGerantProfile(): Observable<GerantProfile> {
    return this.http.get<GerantProfile>(`${this.baseUrl}/gerant/profile`);
  }

  // --- Transporteur endpoints ---

  getTransporteurDashboard(): Observable<TransporteurDashboard> {
    return this.http.get<TransporteurDashboard>(`${this.baseUrl}/transporteur/dashboard`);
  }

  getTransporteurProfile(): Observable<TransporteurProfile> {
    return this.http.get<TransporteurProfile>(`${this.baseUrl}/transporteur/profile`);
  }

  updateAvailability(isAvailable: boolean): Observable<unknown> {
    return this.http.put(`${this.baseUrl}/transporteur/availability`, { isAvailable });
  }

  // --- Custom Auth endpoints ---

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/auth/login`, credentials);
  }

  registerAdminFull(data: any): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/auth/register/admin`, data);
  }

  registerGerantFull(data: any): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/auth/register/gerant`, data);
  }

  registerTransporteurFull(data: any): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/auth/register/transporteur`, data);
  }
}
