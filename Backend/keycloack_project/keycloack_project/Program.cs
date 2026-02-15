using System.Security.Claims;
using keycloack_project.Authorization;
using keycloack_project.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// KEYCLOAK JWT AUTHENTICATION SETUP
// =============================================================================
// Keycloak issues JWT tokens. The backend validates these tokens on every request.
// The token contains: user identity, roles, email, name, etc.
//
// Flow:
// 1. User logs in via Keycloak (through Angular frontend)
// 2. Keycloak issues a JWT access token
// 3. Angular sends this token in the Authorization header: "Bearer <token>"
// 4. ASP.NET validates the token signature, issuer, and audience
// 5. Claims are extracted and mapped to .NET ClaimsPrincipal

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var keycloakConfig = builder.Configuration.GetSection("Keycloak");

    // The Authority is the Keycloak realm URL - used to fetch signing keys
    options.Authority = keycloakConfig["Authority"];

    // The Audience must match the client ID in Keycloak
    options.Audience = keycloakConfig["Audience"];

    // In development, Keycloak runs on HTTP (not HTTPS)
    options.RequireHttpsMetadata = bool.Parse(keycloakConfig["RequireHttpsMetadata"] ?? "true");

    // Where to find the OpenID Connect discovery document
    options.MetadataAddress = keycloakConfig["MetadataAddress"]!;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = keycloakConfig["ValidIssuer"],
        ValidateAudience = true,
        ValidAudiences = new[] { keycloakConfig["Audience"]!, "account" },
        ValidateLifetime = true,
        // Keycloak uses "sub" for the user ID
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});

// Register the claims transformer that converts Keycloak's nested role format
// into flat .NET role claims
builder.Services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

// =============================================================================
// AUTHORIZATION POLICIES
// =============================================================================
// Policies define which roles can access which endpoints.
// Use [Authorize(Policy = "AdminOnly")] on controllers/actions.

builder.Services.AddAuthorization(options =>
{
    // Single-role policies
    options.AddPolicy(AuthPolicies.AdminOnly, policy =>
        policy.RequireRole(KeycloakRoles.Admin));

    options.AddPolicy(AuthPolicies.GerantOnly, policy =>
        policy.RequireRole(KeycloakRoles.Gerant));

    options.AddPolicy(AuthPolicies.TransporteurOnly, policy =>
        policy.RequireRole(KeycloakRoles.Transporteur));

    // Combined policies - admin OR gerant can access
    options.AddPolicy(AuthPolicies.AdminOrGerant, policy =>
        policy.RequireRole(KeycloakRoles.Admin, KeycloakRoles.Gerant));

    // Any authenticated user
    options.AddPolicy(AuthPolicies.AllAuthenticated, policy =>
        policy.RequireAuthenticatedUser());
});

// =============================================================================
// SERVICES
// =============================================================================

// Register UserService as Singleton (in-memory storage for demo)
// In production, use Scoped with a real database
builder.Services.AddSingleton<UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =============================================================================
// CORS CONFIGURATION
// =============================================================================
// Allow Angular frontend to call the API

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection disabled for development - frontend uses HTTP
// app.UseHttpsRedirection();

// IMPORTANT: CORS must come before Authentication/Authorization
app.UseCors("AllowAngular");

// IMPORTANT: Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
