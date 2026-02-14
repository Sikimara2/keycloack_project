using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace keycloack_project.Tests.Helpers;

/// <summary>
/// A fake authentication handler for testing.
/// Instead of validating a real JWT from Keycloak, this handler creates
/// a ClaimsPrincipal with whatever claims we specify in the test.
///
/// This lets us test authorization (role checks) without needing a running Keycloak server.
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthScheme = "TestScheme";

    // Static claims that tests can set before making requests
    public static List<Claim> TestClaims { get; set; } = new();
    public static bool ShouldAuthenticate { get; set; } = true;

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!ShouldAuthenticate)
        {
            return Task.FromResult(AuthenticateResult.Fail("Test: not authenticated"));
        }

        var identity = new ClaimsIdentity(TestClaims, AuthScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <summary>
    /// Helper to create claims for a specific role.
    /// Simulates what KeycloakClaimsTransformation would produce.
    /// </summary>
    public static void SetupUserWithRole(string role, string userId = "test-user-id",
        string email = "test@example.com", string firstName = "Test", string lastName = "User")
    {
        ShouldAuthenticate = true;
        TestClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new("sub", userId),
            new(ClaimTypes.Email, email),
            new("email", email),
            new(ClaimTypes.GivenName, firstName),
            new("given_name", firstName),
            new(ClaimTypes.Surname, lastName),
            new("family_name", lastName),
            new("preferred_username", email),
            new(ClaimTypes.Role, role)
        };
    }

    public static void SetupAnonymous()
    {
        ShouldAuthenticate = false;
        TestClaims = new List<Claim>();
    }

    public static void Reset()
    {
        ShouldAuthenticate = true;
        TestClaims = new List<Claim>();
    }
}
