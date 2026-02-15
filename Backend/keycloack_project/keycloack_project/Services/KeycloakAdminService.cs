using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace keycloack_project.Services;

/// <summary>
/// Service to interact with Keycloak Admin API for user management.
/// Handles user creation, role assignment, and authentication via direct grant.
/// </summary>
public class KeycloakAdminService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _keycloakUrl;
    private readonly string _realm;
    private readonly string _adminUsername;
    private readonly string _adminPassword;
    private readonly string _clientId;

    public KeycloakAdminService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _keycloakUrl = configuration["Keycloak:Url"] ?? "http://localhost:8080";
        _realm = configuration["Keycloak:Realm"] ?? "transport-realm";
        _adminUsername = configuration["Keycloak:AdminUsername"] ?? "admin";
        _adminPassword = configuration["Keycloak:AdminPassword"] ?? "admin";
        _clientId = configuration["Keycloak:ClientId"] ?? "transport-app";
    }

    /// <summary>
    /// Get admin access token for Keycloak Admin API calls
    /// </summary>
    private async Task<string> GetAdminAccessTokenAsync()
    {
        var tokenUrl = $"{_keycloakUrl}/realms/master/protocol/openid-connect/token";

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", "admin-cli"),
            new KeyValuePair<string, string>("username", _adminUsername),
            new KeyValuePair<string, string>("password", _adminPassword)
        });

        var response = await _httpClient.PostAsync(tokenUrl, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);
        return tokenResponse?.AccessToken ?? throw new Exception("Failed to get admin token");
    }

    /// <summary>
    /// Create a new user in Keycloak
    /// </summary>
    public async Task<string> CreateUserAsync(string email, string password, string firstName, string lastName)
    {
        var adminToken = await GetAdminAccessTokenAsync();
        var createUserUrl = $"{_keycloakUrl}/admin/realms/{_realm}/users";

        var user = new
        {
            username = email,
            email = email,
            firstName = firstName,
            lastName = lastName,
            enabled = true,
            emailVerified = true,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = password,
                    temporary = false
                }
            }
        };

        var json = JsonSerializer.Serialize(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var response = await _httpClient.PostAsync(createUserUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create user in Keycloak: {error}");
        }

        // Extract user ID from Location header
        var location = response.Headers.Location?.ToString();
        if (location == null)
            throw new Exception("Failed to get user ID from Keycloak response");

        var userId = location.Split('/').Last();
        return userId;
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    public async Task AssignRoleToUserAsync(string userId, string roleName)
    {
        var adminToken = await GetAdminAccessTokenAsync();

        // First, get the role ID
        var rolesUrl = $"{_keycloakUrl}/admin/realms/{_realm}/roles/{roleName}";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var roleResponse = await _httpClient.GetAsync(rolesUrl);
        roleResponse.EnsureSuccessStatusCode();

        var roleJson = await roleResponse.Content.ReadAsStringAsync();
        var role = JsonSerializer.Deserialize<KeycloakRole>(roleJson);

        if (role == null)
            throw new Exception($"Role {roleName} not found");

        // Assign the role to the user
        var assignRoleUrl = $"{_keycloakUrl}/admin/realms/{_realm}/users/{userId}/role-mappings/realm";
        var roleAssignment = new[] { role };
        var content = new StringContent(JsonSerializer.Serialize(roleAssignment), Encoding.UTF8, "application/json");

        var assignResponse = await _httpClient.PostAsync(assignRoleUrl, content);
        assignResponse.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Authenticate a user using direct password grant (for custom login)
    /// </summary>
    public async Task<TokenResponse> AuthenticateUserAsync(string email, string password)
    {
        var tokenUrl = $"{_keycloakUrl}/realms/{_realm}/protocol/openid-connect/token";

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("username", email),
            new KeyValuePair<string, string>("password", password),
            new KeyValuePair<string, string>("scope", "openid profile email")
        });

        var response = await _httpClient.PostAsync(tokenUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Authentication failed: {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return tokenResponse ?? throw new Exception("Failed to parse token response");
    }
}

// Helper classes for JSON deserialization
public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; } = string.Empty;
}

public class KeycloakRole
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
