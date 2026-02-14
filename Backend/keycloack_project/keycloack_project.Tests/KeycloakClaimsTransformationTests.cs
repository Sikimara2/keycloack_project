using System.Security.Claims;
using keycloack_project.Authorization;
using Xunit;

namespace keycloack_project.Tests;

/// <summary>
/// Unit tests for KeycloakClaimsTransformation.
/// Tests that Keycloak's nested role JSON is correctly transformed into .NET role claims.
/// </summary>
public class KeycloakClaimsTransformationTests
{
    private readonly KeycloakClaimsTransformation _transformer = new();

    [Fact]
    public async Task TransformAsync_WithRealmAccess_ExtractsRoles()
    {
        // Simulate what Keycloak puts in the JWT token
        var realmAccessJson = """{"roles": ["admin", "default-roles-transport-realm"]}""";
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("realm_access", realmAccessJson)
        }, "Bearer");
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformer.TransformAsync(principal);

        var roles = result.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        Assert.Contains("admin", roles);
        Assert.Contains("default-roles-transport-realm", roles);
    }

    [Fact]
    public async Task TransformAsync_WithResourceAccess_ExtractsClientRoles()
    {
        var resourceAccessJson = """{"transport-app": {"roles": ["gerant"]}, "account": {"roles": ["view-profile"]}}""";
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("resource_access", resourceAccessJson)
        }, "Bearer");
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformer.TransformAsync(principal);

        var roles = result.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        Assert.Contains("gerant", roles);
        Assert.Contains("view-profile", roles);
    }

    [Fact]
    public async Task TransformAsync_WithBothRealmAndResourceAccess_CombinesRoles()
    {
        var realmAccessJson = """{"roles": ["admin"]}""";
        var resourceAccessJson = """{"transport-app": {"roles": ["transporteur"]}}""";
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("realm_access", realmAccessJson),
            new Claim("resource_access", resourceAccessJson)
        }, "Bearer");
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformer.TransformAsync(principal);

        var roles = result.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        Assert.Contains("admin", roles);
        Assert.Contains("transporteur", roles);
    }

    [Fact]
    public async Task TransformAsync_WithNoRoleClaims_ReturnsEmptyRoles()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("sub", "user-id")
        }, "Bearer");
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformer.TransformAsync(principal);

        var roles = result.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        Assert.Empty(roles);
    }

    [Fact]
    public async Task TransformAsync_WithMalformedJson_DoesNotThrow()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("realm_access", "not-valid-json")
        }, "Bearer");
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformer.TransformAsync(principal);

        // Should not throw, just skip the malformed claim
        Assert.NotNull(result);
    }

    [Fact]
    public async Task TransformAsync_NoDuplicateRoles()
    {
        var realmAccessJson = """{"roles": ["admin"]}""";
        var resourceAccessJson = """{"transport-app": {"roles": ["admin"]}}""";
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("realm_access", realmAccessJson),
            new Claim("resource_access", resourceAccessJson)
        }, "Bearer");
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformer.TransformAsync(principal);

        var adminRoles = result.FindAll(ClaimTypes.Role)
            .Where(c => c.Value == "admin").ToList();
        Assert.Single(adminRoles);
    }

    [Fact]
    public async Task TransformAsync_UnauthenticatedPrincipal_ReturnsUnchanged()
    {
        var identity = new ClaimsIdentity(); // Not authenticated (no auth type)
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformer.TransformAsync(principal);

        Assert.False(result.Identity?.IsAuthenticated);
    }
}
