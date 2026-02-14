# Keycloak + Angular + ASP.NET - Transport Management Demo

A learning project demonstrating how to integrate **Keycloak** (Identity Provider) with an **Angular 21** frontend and **ASP.NET Core 8** backend, using role-based access control (RBAC).

## What is Keycloak?

Keycloak is an open-source Identity and Access Management (IAM) solution. It handles:
- **User registration and login** (you don't build your own auth system)
- **Single Sign-On (SSO)** across multiple apps
- **Role management** (admin, gerant, transporteur)
- **Token issuance** (JWT tokens for API authentication)
- **User federation** (connect to LDAP, Active Directory, etc.)

### Why use Keycloak instead of custom auth?
- Security: Battle-tested, handles password hashing, brute force protection, etc.
- Standards: Implements OpenID Connect and OAuth 2.0
- Features: MFA, social login, account management come built-in
- Separation of concerns: Your app handles business logic, Keycloak handles identity

## Architecture

```
┌─────────────┐     ┌──────────────┐     ┌─────────────────┐
│   Angular    │────>│   Keycloak   │     │   ASP.NET API   │
│  Frontend    │<────│   (Auth)     │     │   (Backend)     │
│ port: 4200   │     │  port: 8080  │     │  port: 7234     │
└──────┬───────┘     └──────────────┘     └────────┬────────┘
       │                                           │
       │         JWT Token in Header               │
       └───────────────────────────────────────────┘
```

### Authentication Flow
1. User clicks "Login" in Angular app
2. Angular redirects to Keycloak login page
3. User enters credentials on Keycloak
4. Keycloak validates and redirects back with JWT tokens
5. Angular stores the tokens and sends them with every API request
6. ASP.NET validates the JWT token and checks roles
7. API returns data based on the user's role

## Roles

| Role | Description | Specific Fields |
|------|-------------|-----------------|
| **Admin** | Full system access | Department, EmployeeId, CanManageUsers, CanViewReports, CanManageSettings |
| **Gerant** (Manager) | Manages transporteurs in a zone | Zone, PhoneNumber, MaxTransporteursManaged, IsActive |
| **Transporteur** (Driver) | Handles deliveries | LicenseNumber, VehicleType, VehiclePlate, PhoneNumber, IsAvailable, AssignedGerantId |

**Common fields** (stored in Keycloak): FirstName, LastName, Email, Username

## Quick Start

### 1. Start Keycloak (Docker)
```bash
docker-compose up -d
```
This starts Keycloak on http://localhost:8080 with pre-configured:
- Realm: `transport-realm`
- Client: `transport-app`
- 3 roles: admin, gerant, transporteur
- 3 test users (see below)

### 2. Start the Backend
```bash
cd Backend/keycloack_project/keycloack_project
dotnet run
```
API runs on https://localhost:7234

### 3. Start the Frontend
```bash
cd Angular/my-keycloak-frontend
npm install
npm start
```
App runs on http://localhost:4200

### Test Users

| Email | Password | Role |
|-------|----------|------|
| admin@transport.com | admin123 | admin |
| gerant@transport.com | gerant123 | gerant |
| transporteur@transport.com | transporteur123 | transporteur |

### Keycloak Admin Console
- URL: http://localhost:8080
- Username: admin
- Password: admin

## Project Structure

```
├── docker-compose.yml              # Keycloak + PostgreSQL
├── keycloak-config/
│   └── transport-realm.json        # Pre-configured realm with roles & users
│
├── Backend/keycloack_project/keycloack_project/
│   ├── Program.cs                  # App config: JWT auth, policies, CORS
│   ├── Authorization/
│   │   ├── KeycloakRoles.cs        # Role & policy constants
│   │   └── KeycloakClaimsTransformation.cs  # Maps Keycloak JWT roles to .NET claims
│   ├── Controllers/
│   │   ├── AdminController.cs      # [Authorize(Policy = "AdminOnly")]
│   │   ├── GerantController.cs     # [Authorize(Policy = "GerantOnly")]
│   │   ├── TransporteurController.cs # [Authorize(Policy = "TransporteurOnly")]
│   │   └── ProfileController.cs    # [Authorize] - any authenticated user
│   ├── Models/
│   │   └── UserProfile.cs          # Base + role-specific profiles
│   ├── DTOs/
│   │   └── UserDtos.cs             # Request/Response DTOs
│   └── Services/
│       └── UserService.cs          # In-memory user store (replace with DB)
│
├── Backend/keycloack_project/keycloack_project.Tests/
│   ├── AdminControllerAuthTests.cs         # Tests admin endpoints block other roles
│   ├── GerantControllerAuthTests.cs        # Tests gerant endpoints block other roles
│   ├── TransporteurControllerAuthTests.cs  # Tests transporteur endpoints block other roles
│   ├── ProfileControllerAuthTests.cs       # Tests profile endpoints & role-specific registration
│   ├── KeycloakClaimsTransformationTests.cs # Tests JWT claims parsing
│   └── Helpers/
│       ├── TestAuthHandler.cs              # Fake auth for testing without Keycloak
│       └── TestWebApplicationFactory.cs    # Test server factory
│
└── Angular/my-keycloak-frontend/
    └── src/app/
        ├── app.config.ts           # APP_INITIALIZER for Keycloak
        ├── app.routes.ts           # Routes with auth guards
        ├── services/
        │   ├── keycloak.service.ts # Keycloak JS adapter wrapper
        │   └── api.service.ts      # HTTP calls to backend
        ├── guards/
        │   └── auth.guard.ts       # Route guard checking auth + roles
        ├── interceptors/
        │   └── auth.interceptor.ts # Auto-attaches JWT to API requests
        ├── models/
        │   └── user.model.ts       # TypeScript interfaces
        ├── components/
        │   └── navbar/             # Navigation with role display
        └── pages/
            ├── login/              # Custom login page (redirects to Keycloak)
            ├── register/           # Role-specific profile registration
            ├── dashboard/          # Role-based dashboard routing
            │   ├── admin-dashboard/
            │   ├── gerant-dashboard/
            │   └── transporteur-dashboard/
            ├── home/
            └── unauthorized/

```

## Key Concepts Explained

### Backend: How JWT Validation Works

```csharp
// In Program.cs - Configure JWT Bearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/transport-realm";
        options.Audience = "transport-app";
        // ASP.NET automatically:
        // 1. Fetches Keycloak's public keys from the discovery endpoint
        // 2. Validates the token signature
        // 3. Checks the issuer and audience
        // 4. Checks token expiration
    });
```

### Backend: How Role Authorization Works

```csharp
// Keycloak JWT contains nested roles:
// { "realm_access": { "roles": ["admin"] } }
//
// KeycloakClaimsTransformation converts this to flat .NET claims:
// ClaimTypes.Role = "admin"
//
// Then we can use standard .NET authorization:
[Authorize(Policy = "AdminOnly")]  // Only admin role can access
public class AdminController : ControllerBase { }
```

### Frontend: How the Auth Interceptor Works

```typescript
// Every HTTP request passes through the interceptor
// It automatically adds: Authorization: Bearer <jwt-token>
export const authInterceptor: HttpInterceptorFn = (req, next) => {
    return from(keycloakService.getToken()).pipe(
        switchMap((token) => {
            const authReq = req.clone({
                setHeaders: { Authorization: `Bearer ${token}` },
            });
            return next(authReq);
        })
    );
};
```

### Frontend: How Route Guards Work

```typescript
// Guards run before a route activates
export const authGuard: CanActivateFn = (route) => {
    if (!keycloakService.isAuthenticated()) {
        return router.createUrlTree(['/login']);  // Redirect to login
    }
    const requiredRole = route.data?.['role'];
    if (requiredRole && !keycloakService.hasRole(requiredRole)) {
        return router.createUrlTree(['/unauthorized']);  // Wrong role
    }
    return true;  // Allow access
};
```

## API Endpoints

### Profile (Any Authenticated User)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/profile/me` | Get user info from JWT token |
| POST | `/api/profile/register/admin` | Register admin-specific data (admin only) |
| POST | `/api/profile/register/gerant` | Register gerant-specific data (gerant only) |
| POST | `/api/profile/register/transporteur` | Register transporteur-specific data (transporteur only) |

### Admin Endpoints (Admin Only)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/dashboard` | System stats |
| GET | `/api/admin/users` | List all users |
| GET | `/api/admin/gerants` | List all gerants |
| GET | `/api/admin/transporteurs` | List all transporteurs |
| POST | `/api/admin/assign-transporteur` | Assign transporteur to gerant |

### Gerant Endpoints (Gerant Only)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/gerant/dashboard` | Manager stats |
| GET | `/api/gerant/my-transporteurs` | Transporteurs assigned to this gerant |
| GET | `/api/gerant/profile` | Gerant's own profile |

### Transporteur Endpoints (Transporteur Only)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/transporteur/dashboard` | Driver dashboard |
| GET | `/api/transporteur/profile` | Driver's own profile |
| PUT | `/api/transporteur/availability` | Toggle availability |

## Running Tests

```bash
cd Backend/keycloack_project
dotnet test
```

Tests verify that:
- Admin endpoints return 200 for admin, 403 for gerant/transporteur, 401 for anonymous
- Gerant endpoints return 200 for gerant, 403 for admin/transporteur, 401 for anonymous
- Transporteur endpoints return 200 for transporteur, 403 for admin/gerant, 401 for anonymous
- Profile `/me` endpoint works for all authenticated users
- Registration endpoints enforce role-specific access
- Keycloak claims transformation correctly parses JWT roles
