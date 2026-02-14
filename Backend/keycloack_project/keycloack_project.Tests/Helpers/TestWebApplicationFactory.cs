using keycloack_project.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace keycloack_project.Tests.Helpers;

/// <summary>
/// Custom WebApplicationFactory for integration tests.
///
/// This replaces the real Keycloak JWT authentication with our TestAuthHandler,
/// so we can test authorization policies without a running Keycloak server.
///
/// The authorization policies (AdminOnly, GerantOnly, etc.) still work because
/// they check ClaimTypes.Role claims, which our TestAuthHandler provides.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real JWT authentication and replace with test auth
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.AuthScheme;
                options.DefaultChallengeScheme = TestAuthHandler.AuthScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.AuthScheme, _ => { });

            // Re-register authorization policies (they use the same role claims)
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicies.AdminOnly, policy =>
                    policy.RequireRole(KeycloakRoles.Admin));

                options.AddPolicy(AuthPolicies.GerantOnly, policy =>
                    policy.RequireRole(KeycloakRoles.Gerant));

                options.AddPolicy(AuthPolicies.TransporteurOnly, policy =>
                    policy.RequireRole(KeycloakRoles.Transporteur));

                options.AddPolicy(AuthPolicies.AdminOrGerant, policy =>
                    policy.RequireRole(KeycloakRoles.Admin, KeycloakRoles.Gerant));

                options.AddPolicy(AuthPolicies.AllAuthenticated, policy =>
                    policy.RequireAuthenticatedUser());
            });
        });

        builder.UseEnvironment("Development");
    }
}
