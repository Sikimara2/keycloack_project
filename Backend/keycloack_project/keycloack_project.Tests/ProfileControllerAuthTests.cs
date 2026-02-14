using System.Net;
using System.Text;
using System.Text.Json;
using keycloack_project.Tests.Helpers;
using Xunit;

namespace keycloack_project.Tests;

/// <summary>
/// Tests that ProfileController endpoints respect authorization:
/// - /api/profile/me is accessible by ANY authenticated user
/// - /api/profile/register/admin requires admin role
/// - /api/profile/register/gerant requires gerant role
/// - /api/profile/register/transporteur requires transporteur role
/// </summary>
public class ProfileControllerAuthTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;

    public ProfileControllerAuthTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    public void Dispose()
    {
        TestAuthHandler.Reset();
    }

    // =========================================================================
    // /api/profile/me - ANY AUTHENTICATED USER
    // =========================================================================

    [Theory]
    [InlineData("admin")]
    [InlineData("gerant")]
    [InlineData("transporteur")]
    public async Task GetMe_WithAnyRole_Returns200(string role)
    {
        TestAuthHandler.SetupUserWithRole(role);

        var response = await _client.GetAsync("/api/profile/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_Anonymous_Returns401()
    {
        TestAuthHandler.SetupAnonymous();

        var response = await _client.GetAsync("/api/profile/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_ReturnsUserClaimsFromToken()
    {
        TestAuthHandler.SetupUserWithRole("admin", "kc-123", "admin@test.com", "John", "Doe");

        var response = await _client.GetAsync("/api/profile/me");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("admin@test.com", content);
        Assert.Contains("John", content);
        Assert.Contains("Doe", content);
    }

    // =========================================================================
    // /api/profile/register/admin - ADMIN ONLY
    // =========================================================================

    [Fact]
    public async Task RegisterAdmin_WithAdminRole_IsAuthorized()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var body = new StringContent(
            JsonSerializer.Serialize(new { Department = "IT", EmployeeId = "EMP001" }),
            Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/profile/register/admin", body);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RegisterAdmin_WithGerantRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var body = new StringContent(
            JsonSerializer.Serialize(new { Department = "IT", EmployeeId = "EMP001" }),
            Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/profile/register/admin", body);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RegisterAdmin_WithTransporteurRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("transporteur");

        var body = new StringContent(
            JsonSerializer.Serialize(new { Department = "IT", EmployeeId = "EMP001" }),
            Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/profile/register/admin", body);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // =========================================================================
    // /api/profile/register/gerant - GERANT ONLY
    // =========================================================================

    [Fact]
    public async Task RegisterGerant_WithGerantRole_IsAuthorized()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var body = new StringContent(
            JsonSerializer.Serialize(new { Zone = "North", PhoneNumber = "+33612345678" }),
            Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/profile/register/gerant", body);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RegisterGerant_WithAdminRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var body = new StringContent(
            JsonSerializer.Serialize(new { Zone = "North", PhoneNumber = "+33612345678" }),
            Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/profile/register/gerant", body);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // =========================================================================
    // /api/profile/register/transporteur - TRANSPORTEUR ONLY
    // =========================================================================

    [Fact]
    public async Task RegisterTransporteur_WithTransporteurRole_IsAuthorized()
    {
        TestAuthHandler.SetupUserWithRole("transporteur");

        var body = new StringContent(
            JsonSerializer.Serialize(new
            {
                LicenseNumber = "DL-123456",
                VehicleType = "Van",
                VehiclePlate = "AB-123-CD",
                PhoneNumber = "+33612345678"
            }),
            Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/profile/register/transporteur", body);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RegisterTransporteur_WithAdminRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("admin");

        var body = new StringContent(
            JsonSerializer.Serialize(new
            {
                LicenseNumber = "DL-123456",
                VehicleType = "Van",
                VehiclePlate = "AB-123-CD",
                PhoneNumber = "+33612345678"
            }),
            Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/profile/register/transporteur", body);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RegisterTransporteur_WithGerantRole_Returns403()
    {
        TestAuthHandler.SetupUserWithRole("gerant");

        var body = new StringContent(
            JsonSerializer.Serialize(new
            {
                LicenseNumber = "DL-123456",
                VehicleType = "Van",
                VehiclePlate = "AB-123-CD",
                PhoneNumber = "+33612345678"
            }),
            Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/profile/register/transporteur", body);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
