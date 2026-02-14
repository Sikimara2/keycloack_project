using System.Net;
using keycloack_project.Tests.Helpers;
using Xunit;

namespace keycloack_project.Tests;

/// <summary>
/// Tests that AdminController endpoints are only accessible by users with the "admin" role.
///
/// WHAT WE'RE TESTING:
/// - Admin endpoints return 200 when called with admin role
/// - Admin endpoints return 403 (Forbidden) when called with gerant role
/// - Admin endpoints return 403 (Forbidden) when called with transporteur role
/// - Admin endpoints return 401 (Unauthorized) when called without authentication
/// </summary>
public class AdminControllerAuthTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;

    public AdminControllerAuthTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    public void Dispose()
    {
        TestAuthHandler.Reset();
    }

    // =========================================================================
    // ADMIN ROLE - SHOULD SUCCEED (200 OK)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_WithAdminRole_Returns200()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var response = await _client.GetAsync("/api/admin/dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_WithAdminRole_Returns200()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var response = await _client.GetAsync("/api/admin/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetGerants_WithAdminRole_Returns200()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var response = await _client.GetAsync("/api/admin/gerants");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTransporteurs_WithAdminRole_Returns200()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var response = await _client.GetAsync("/api/admin/transporteurs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // =========================================================================
    // GERANT ROLE - SHOULD BE FORBIDDEN (403)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_WithGerantRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var response = await _client.GetAsync("/api/admin/dashboard");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_WithGerantRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var response = await _client.GetAsync("/api/admin/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetGerants_WithGerantRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var response = await _client.GetAsync("/api/admin/gerants");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // =========================================================================
    // TRANSPORTEUR ROLE - SHOULD BE FORBIDDEN (403)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_WithTransporteurRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("transporteur");

        var response = await _client.GetAsync("/api/admin/dashboard");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_WithTransporteurRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("transporteur");

        var response = await _client.GetAsync("/api/admin/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // =========================================================================
    // ANONYMOUS (NO AUTH) - SHOULD BE UNAUTHORIZED (401)
    // =========================================================================

    [Fact]
    public async Task GetDashboard_Anonymous_Returns401()
    {
        TestAuthHandler.SetupAnonymous();

        var response = await _client.GetAsync("/api/admin/dashboard");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_Anonymous_Returns401()
    {
        TestAuthHandler.SetupAnonymous();

        var response = await _client.GetAsync("/api/admin/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
