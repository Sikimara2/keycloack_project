using System.Security.Claims;
using keycloack_project.Authorization;
using keycloack_project.DTOs;
using keycloack_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keycloack_project.Controllers;

/// <summary>
/// Gerant (Manager) endpoints.
/// Only users with the "gerant" role in Keycloak can access these.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthPolicies.GerantOnly)]
public class GerantController : ControllerBase
{
    private readonly UserService _userService;

    public GerantController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// GET /api/gerant/dashboard - Gerant dashboard with managed transporteurs stats.
    /// </summary>
    [HttpGet("dashboard")]
    public ActionResult<GerantDashboardDto> GetDashboard()
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (keycloakId == null) return Unauthorized();

        var gerant = _userService.GetGerantByKeycloakId(keycloakId);
        if (gerant == null) return NotFound("Gerant profile not found");

        return Ok(_userService.GetGerantDashboard(gerant.Id));
    }

    /// <summary>
    /// GET /api/gerant/my-transporteurs - List transporteurs assigned to this gerant.
    /// </summary>
    [HttpGet("my-transporteurs")]
    public ActionResult<List<TransporteurProfileDto>> GetMyTransporteurs()
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (keycloakId == null) return Unauthorized();

        var gerant = _userService.GetGerantByKeycloakId(keycloakId);
        if (gerant == null) return NotFound("Gerant profile not found");

        var transporteurs = _userService.GetTransporteursByGerant(gerant.Id)
            .Select(t => new TransporteurProfileDto
            {
                Id = t.Id, FirstName = t.FirstName, LastName = t.LastName,
                Email = t.Email, Role = t.Role, CreatedAt = t.CreatedAt,
                LicenseNumber = t.LicenseNumber, VehicleType = t.VehicleType,
                VehiclePlate = t.VehiclePlate, PhoneNumber = t.PhoneNumber,
                IsAvailable = t.IsAvailable, AssignedGerantId = t.AssignedGerantId
            }).ToList();

        return Ok(transporteurs);
    }

    /// <summary>
    /// GET /api/gerant/profile - Get current gerant's profile.
    /// </summary>
    [HttpGet("profile")]
    public ActionResult<GerantProfileDto> GetProfile()
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (keycloakId == null) return Unauthorized();

        var gerant = _userService.GetGerantByKeycloakId(keycloakId);
        if (gerant == null) return NotFound("Gerant profile not found");

        return Ok(new GerantProfileDto
        {
            Id = gerant.Id, FirstName = gerant.FirstName, LastName = gerant.LastName,
            Email = gerant.Email, Role = gerant.Role, CreatedAt = gerant.CreatedAt,
            Zone = gerant.Zone, PhoneNumber = gerant.PhoneNumber,
            MaxTransporteursManaged = gerant.MaxTransporteursManaged, IsActive = gerant.IsActive
        });
    }
}
