using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace keycloack_project.Authorization;

/// <summary>
/// Transforms Keycloak JWT token claims into .NET role claims.
///
/// HOW IT WORKS:
/// Keycloak stores roles in a nested JSON structure inside the JWT:
///   "realm_access": { "roles": ["admin", "gerant", "transporteur"] }
///
/// .NET expects flat role claims like:
///   ClaimTypes.Role = "admin"
///
/// This transformer extracts the nested roles and adds them as standard .NET role claims
/// so we can use [Authorize(Roles = "admin")] on controllers.
/// </summary>
public class KeycloakClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null || !identity.IsAuthenticated)
            return Task.FromResult(principal);

        // Extract roles from "realm_access" claim (Keycloak realm roles)
        var realmAccessClaim = identity.FindFirst("realm_access");
        if (realmAccessClaim != null)
        {
            AddRolesFromJson(identity, realmAccessClaim.Value);
        }

        // Also extract from "resource_access" for client-specific roles
        var resourceAccessClaim = identity.FindFirst("resource_access");
        if (resourceAccessClaim != null)
        {
            try
            {
                using var doc = JsonDocument.Parse(resourceAccessClaim.Value);
                foreach (var client in doc.RootElement.EnumerateObject())
                {
                    if (client.Value.TryGetProperty("roles", out var roles))
                    {
                        foreach (var role in roles.EnumerateArray())
                        {
                            var roleValue = role.GetString();
                            if (!string.IsNullOrEmpty(roleValue) &&
                                !identity.HasClaim(ClaimTypes.Role, roleValue))
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Malformed resource_access claim - skip
            }
        }

        return Task.FromResult(principal);
    }

    private static void AddRolesFromJson(ClaimsIdentity identity, string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("roles", out var roles))
            {
                foreach (var role in roles.EnumerateArray())
                {
                    var roleValue = role.GetString();
                    if (!string.IsNullOrEmpty(roleValue) &&
                        !identity.HasClaim(ClaimTypes.Role, roleValue))
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Malformed realm_access claim - skip
        }
    }
}
