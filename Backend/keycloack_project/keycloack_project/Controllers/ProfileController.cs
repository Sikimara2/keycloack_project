using System.Security.Claims;
using keycloack_project.Authorization;
using keycloack_project.DTOs;
using keycloack_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keycloack_project.Controllers;

/// <summary>
/// Shared endpoints accessible by all authenticated users.
/// Used for profile registration and viewing own profile.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserService _userService;

    public ProfileController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// GET /api/profile/me - Get the current user's info from the JWT token.
    /// This shows how to extract Keycloak user data from the token.
    /// </summary>
    [HttpGet("me")]
    public ActionResult GetMyProfile()
    {
        // These claims come directly from the Keycloak JWT token
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value;
        var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value
            ?? User.FindFirst("given_name")?.Value;
        var lastName = User.FindFirst(ClaimTypes.Surname)?.Value
            ?? User.FindFirst("family_name")?.Value;
        var preferredUsername = User.FindFirst("preferred_username")?.Value;

        // Get all roles from the token
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            keycloakId,
            email,
            firstName,
            lastName,
            preferredUsername,
            roles
        });
    }

    /// <summary>
    /// POST /api/profile/register/admin - Register admin-specific profile data.
    /// The user must already have the "admin" role in Keycloak.
    /// </summary>
    [HttpPost("register/admin")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public ActionResult<AdminProfileDto> RegisterAdmin([FromBody] RegisterAdminDto dto)
    {
        var (keycloakId, firstName, lastName, email) = ExtractUserClaims();
        if (keycloakId == null) return Unauthorized();

        var admin = _userService.CreateAdmin(keycloakId, firstName, lastName, email, dto);
        return CreatedAtAction(nameof(GetMyProfile), new AdminProfileDto
        {
            Id = admin.Id, FirstName = admin.FirstName, LastName = admin.LastName,
            Email = admin.Email, Role = admin.Role, CreatedAt = admin.CreatedAt,
            Department = admin.Department, EmployeeId = admin.EmployeeId,
            CanManageUsers = admin.CanManageUsers, CanViewReports = admin.CanViewReports,
            CanManageSettings = admin.CanManageSettings
        });
    }

    /// <summary>
    /// POST /api/profile/register/gerant - Register gerant-specific profile data.
    /// </summary>
    [HttpPost("register/gerant")]
    [Authorize(Policy = AuthPolicies.GerantOnly)]
    public ActionResult<GerantProfileDto> RegisterGerant([FromBody] RegisterGerantDto dto)
    {
        var (keycloakId, firstName, lastName, email) = ExtractUserClaims();
        if (keycloakId == null) return Unauthorized();

        var gerant = _userService.CreateGerant(keycloakId, firstName, lastName, email, dto);
        return CreatedAtAction(nameof(GetMyProfile), new GerantProfileDto
        {
            Id = gerant.Id, FirstName = gerant.FirstName, LastName = gerant.LastName,
            Email = gerant.Email, Role = gerant.Role, CreatedAt = gerant.CreatedAt,
            Zone = gerant.Zone, PhoneNumber = gerant.PhoneNumber,
            MaxTransporteursManaged = gerant.MaxTransporteursManaged, IsActive = gerant.IsActive
        });
    }

    /// <summary>
    /// POST /api/profile/register/transporteur - Register transporteur-specific profile data.
    /// </summary>
    [HttpPost("register/transporteur")]
    [Authorize(Policy = AuthPolicies.TransporteurOnly)]
    public ActionResult<TransporteurProfileDto> RegisterTransporteur(
        [FromBody] RegisterTransporteurDto dto)
    {
        var (keycloakId, firstName, lastName, email) = ExtractUserClaims();
        if (keycloakId == null) return Unauthorized();

        var transporteur = _userService.CreateTransporteur(
            keycloakId, firstName, lastName, email, dto);
        return CreatedAtAction(nameof(GetMyProfile), new TransporteurProfileDto
        {
            Id = transporteur.Id, FirstName = transporteur.FirstName,
            LastName = transporteur.LastName, Email = transporteur.Email,
            Role = transporteur.Role, CreatedAt = transporteur.CreatedAt,
            LicenseNumber = transporteur.LicenseNumber, VehicleType = transporteur.VehicleType,
            VehiclePlate = transporteur.VehiclePlate, PhoneNumber = transporteur.PhoneNumber,
            IsAvailable = transporteur.IsAvailable, AssignedGerantId = transporteur.AssignedGerantId
        });
    }

    private (string? keycloakId, string firstName, string lastName, string email) ExtractUserClaims()
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value
            ?? User.FindFirst("given_name")?.Value ?? "";
        var lastName = User.FindFirst(ClaimTypes.Surname)?.Value
            ?? User.FindFirst("family_name")?.Value ?? "";
        var email = User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value ?? "";
        return (keycloakId, firstName, lastName, email);
    }
}
