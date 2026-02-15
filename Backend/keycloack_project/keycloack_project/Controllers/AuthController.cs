using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using keycloack_project.DTOs;
using keycloack_project.Services;
using Microsoft.AspNetCore.Mvc;

namespace keycloack_project.Controllers;

/// <summary>
/// Authentication Controller - Handles custom login and registration.
/// Uses Keycloak Admin API to create users programmatically.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly KeycloakAdminService _keycloakAdmin;
    private readonly UserService _userService;

    public AuthController(KeycloakAdminService keycloakAdmin, UserService userService)
    {
        _keycloakAdmin = keycloakAdmin;
        _userService = userService;
    }

    /// <summary>
    /// POST /api/auth/login - Custom login endpoint
    /// Authenticates user via Keycloak and returns tokens
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            // Authenticate with Keycloak using direct password grant
            var tokenResponse = await _keycloakAdmin.AuthenticateUserAsync(request.Email, request.Password);

            // Parse the access token to extract user info
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenResponse.AccessToken);

            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? request.Email;
            var firstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? "";
            var lastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? "";

            // Extract roles from realm_access
            var realmAccessClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "realm_access");
            var roles = new List<string>();
            if (realmAccessClaim != null)
            {
                var realmAccess = System.Text.Json.JsonDocument.Parse(realmAccessClaim.Value);
                if (realmAccess.RootElement.TryGetProperty("roles", out var rolesArray))
                {
                    roles = rolesArray.EnumerateArray()
                        .Select(r => r.GetString())
                        .Where(r => r != null)
                        .Cast<string>()
                        .ToList();
                }
            }

            var primaryRole = roles.Contains("admin") ? "admin" :
                             roles.Contains("gerant") ? "gerant" :
                             roles.Contains("transporteur") ? "transporteur" : null;

            return Ok(new LoginResponseDto
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                TokenType = tokenResponse.TokenType,
                UserInfo = new UserInfoDto
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Roles = roles.ToArray(),
                    PrimaryRole = primaryRole
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Invalid credentials", error = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/auth/register/admin - Register a new admin user
    /// Creates user in Keycloak, assigns admin role, and saves profile
    /// </summary>
    [HttpPost("register/admin")]
    public async Task<ActionResult> RegisterAdmin([FromBody] RegisterAdminRequestDto request)
    {
        try
        {
            // Step 1: Create user in Keycloak
            var keycloakUserId = await _keycloakAdmin.CreateUserAsync(
                request.Email, request.Password, request.FirstName, request.LastName);

            // Step 2: Assign admin role
            await _keycloakAdmin.AssignRoleToUserAsync(keycloakUserId, "admin");

            // Step 3: Save profile data in our database
            var admin = _userService.CreateAdminFromFullRegistration(keycloakUserId, request);

            return Ok(new
            {
                message = "Admin registered successfully",
                userId = admin.Id,
                email = admin.Email
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Registration failed", error = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/auth/register/gerant - Register a new gerant (manager) user
    /// </summary>
    [HttpPost("register/gerant")]
    public async Task<ActionResult> RegisterGerant([FromBody] RegisterGerantRequestDto request)
    {
        try
        {
            // Step 1: Create user in Keycloak
            var keycloakUserId = await _keycloakAdmin.CreateUserAsync(
                request.Email, request.Password, request.FirstName, request.LastName);

            // Step 2: Assign gerant role
            await _keycloakAdmin.AssignRoleToUserAsync(keycloakUserId, "gerant");

            // Step 3: Save profile data in our database
            var gerant = _userService.CreateGerantFromFullRegistration(keycloakUserId, request);

            return Ok(new
            {
                message = "Gerant registered successfully",
                userId = gerant.Id,
                email = gerant.Email
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Registration failed", error = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/auth/register/transporteur - Register a new transporteur (driver) user
    /// </summary>
    [HttpPost("register/transporteur")]
    public async Task<ActionResult> RegisterTransporteur([FromBody] RegisterTransporteurRequestDto request)
    {
        try
        {
            // Step 1: Create user in Keycloak
            var keycloakUserId = await _keycloakAdmin.CreateUserAsync(
                request.Email, request.Password, request.FirstName, request.LastName);

            // Step 2: Assign transporteur role
            await _keycloakAdmin.AssignRoleToUserAsync(keycloakUserId, "transporteur");

            // Step 3: Save profile data in our database
            var transporteur = _userService.CreateTransporteurFromFullRegistration(keycloakUserId, request);

            return Ok(new
            {
                message = "Transporteur registered successfully",
                userId = transporteur.Id,
                email = transporteur.Email
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Registration failed", error = ex.Message });
        }
    }
}
