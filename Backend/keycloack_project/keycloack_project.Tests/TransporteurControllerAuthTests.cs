using System.Net;
using System.Text;
using System.Text.Json;
using keycloack_project.Tests.Helpers;
using Xunit;

namespace keycloack_project.Tests;

/// <summary>
/// Tests that TransporteurController endpoints are only accessible by users with the "transporteur" role.
///
/// WHAT WE'RE TESTING:
/// - Transporteur endpoints are authorized when called with transporteur role
/// - Transporteur endpoints return 403 when called with admin role
/// - Transporteur endpoints return 403 when called with gerant role
/// - Transporteur endpoints return 401 when called without authentication
/// </summary>
public class TransporteurControllerAuthTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;

    public TransporteurControllerAuthTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    public void Dispose()
    {
        TestAuthHandler.Reset();
    }

    // =========================================================================
    // TRANSPORTEUR ROLE - SHOULD BE AUTHORIZED
    // =========================================================================

    [Fact]
    public async Task GetDashboard_WithTransporteurRole_IsAuthorized()
    {
        TestAuthHandler.SetupUserWithRole("transporteur");

        var response = await _client.GetAsync("/api/transporteur/dashboard");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithTransporteurRole_IsAuthorized()
    {
        TestAuthHandler.SetupUserWithRole("transporteur");

        var response = await _client.GetAsync("/api/transporteur/profile");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAvailability_WithTransporteurRole_IsAuthorized()
    {
        TestAuthHandler.SetupUserWithRole("transporteur");

        var content = new StringContent(
            JsonSerializer.Serialize(new { IsAvailable = true }),
            Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("/api/transporteur/availability", content);

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

        var response = await _client.GetAsync("/api/transporteur/dashboard");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithAdminRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var response = await _client.GetAsync("/api/transporteur/profile");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAvailability_WithAdminRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var content = new StringContent(
            JsonSerializer.Serialize(new { IsAvailable = true }),
            Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("/api/transporteur/availability", content);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // =========================================================================
    // GERANT ROLE - SHOULD BE FORBIDDEN (403)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_WithGerantRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var response = await _client.GetAsync("/api/transporteur/dashboard");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithGerantRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var response = await _client.GetAsync("/api/transporteur/profile");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // =========================================================================
    // ANONYMOUS (NO AUTH) - SHOULD BE UNAUTHORIZED (401)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_Anonymous_Returns401()
    {
        TestAuthHandler.SetupAnonymous();

        var response = await _client.GetAsync("/api/transporteur/dashboard");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_Anonymous_Returns401()
    {
        TestAuthHandler.SetupAnonymous();

        var response = await _client.GetAsync("/api/transporteur/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAvailability_Anonymous_Returns401()
    {
        TestAuthHandler.SetupAnonymous();

        var content = new StringContent(
            JsonSerializer.Serialize(new { IsAvailable = true }),
            Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("/api/transporteur/availability", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
