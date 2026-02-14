using System.Net;
using keycloack_project.Tests.Helpers;
using Xunit;

namespace keycloack_project.Tests;

/// <summary>
/// Tests that GerantController endpoints are only accessible by users with the "gerant" role.
///
/// WHAT WE'RE TESTING:
/// - Gerant endpoints return 200/404 when called with gerant role (200 = authorized, 404 = profile not found)
/// - Gerant endpoints return 403 when called with admin role
/// - Gerant endpoints return 403 when called with transporteur role
/// - Gerant endpoints return 401 when called without authentication
/// </summary>
public class GerantControllerAuthTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;

    public GerantControllerAuthTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    public void Dispose()
    {
        TestAuthHandler.Reset();
    }

    // =========================================================================
    // GERANT ROLE - SHOULD BE AUTHORIZED (not 401/403)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_WithGerantRole_IsAuthorized()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var response = await _client.GetAsync("/api/gerant/dashboard");

        // Should not be 401 or 403 - authorization passed
        // May return 404 because the gerant profile doesn't exist in the in-memory store
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMyTransporteurs_WithGerantRole_IsAuthorized()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var response = await _client.GetAsync("/api/gerant/my-transporteurs");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithGerantRole_IsAuthorized()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var response = await _client.GetAsync("/api/gerant/profile");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // =========================================================================
    // ADMIN ROLE - SHOULD BE FORBIDDEN (403)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_WithAdminRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var response = await _client.GetAsync("/api/gerant/dashboard");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMyTransporteurs_WithAdminRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var response = await _client.GetAsync("/api/gerant/my-transporteurs");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithAdminRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var response = await _client.GetAsync("/api/gerant/profile");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // =========================================================================
    // TRANSPORTEUR ROLE - SHOULD BE FORBIDDEN (403)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_WithTransporteurRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("transporteur");

        var response = await _client.GetAsync("/api/gerant/dashboard");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMyTransporteurs_WithTransporteurRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("transporteur");

        var response = await _client.GetAsync("/api/gerant/my-transporteurs");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // =========================================================================
    // ANONYMOUS (NO AUTH) - SHOULD BE UNAUTHORIZED (401)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_Anonymous_Returns401()
    {
        TestAuthHandler.SetupAnonymous();

        var response = await _client.GetAsync("/api/gerant/dashboard");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_Anonymous_Returns401()
    {
        TestAuthHandler.SetupAnonymous();

        var response = await _client.GetAsync("/api/gerant/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
