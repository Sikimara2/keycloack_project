using System.Security.Claims;
using keycloack_project.Authorization;
using keycloack_project.DTOs;
using keycloack_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keycloack_project.Controllers;

/// <summary>
/// Transporteur (Driver) endpoints.
/// Only users with the "transporteur" role in Keycloak can access these.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthPolicies.TransporteurOnly)]
public class TransporteurController : ControllerBase
{
    private readonly UserService _userService;

    public TransporteurController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// GET /api/transporteur/dashboard - Transporteur's personal dashboard.
    /// </summary>
    [HttpGet("dashboard")]
    public ActionResult<TransporteurDashboardDto> GetDashboard()
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (keycloakId == null) return Unauthorized();

        var transporteur = _userService.GetTransporteurByKeycloakId(keycloakId);
        if (transporteur == null) return NotFound("Transporteur profile not found");

        var gerantName = "";
        if (transporteur.AssignedGerantId.HasValue)
        {
            var gerant = _userService.GetGerantById(transporteur.AssignedGerantId.Value);
            gerantName = gerant != null ? $"{gerant.FirstName} {gerant.LastName}" : "";
        }

        return Ok(new TransporteurDashboardDto
        {
            IsAvailable = transporteur.IsAvailable,
            VehicleType = transporteur.VehicleType,
            AssignedGerantName = gerantName
        });
    }

    /// <summary>
    /// GET /api/transporteur/profile - Get current transporteur's profile.
    /// </summary>
    [HttpGet("profile")]
    public ActionResult<TransporteurProfileDto> GetProfile()
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (keycloakId == null) return Unauthorized();

        var transporteur = _userService.GetTransporteurByKeycloakId(keycloakId);
        if (transporteur == null) return NotFound("Transporteur profile not found");

        return Ok(new TransporteurProfileDto
        {
            Id = transporteur.Id, FirstName = transporteur.FirstName,
            LastName = transporteur.LastName, Email = transporteur.Email,
            Role = transporteur.Role, CreatedAt = transporteur.CreatedAt,
            LicenseNumber = transporteur.LicenseNumber, VehicleType = transporteur.VehicleType,
            VehiclePlate = transporteur.VehiclePlate, PhoneNumber = transporteur.PhoneNumber,
            IsAvailable = transporteur.IsAvailable, AssignedGerantId = transporteur.AssignedGerantId
        });
    }

    /// <summary>
    /// PUT /api/transporteur/availability - Update own availability status.
    /// </summary>
    [HttpPut("availability")]
    public ActionResult UpdateAvailability([FromBody] UpdateTransporteurAvailabilityDto dto)
    {
        var keycloakId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (keycloakId == null) return Unauthorized();

        var transporteur = _userService.GetTransporteurByKeycloakId(keycloakId);
        if (transporteur == null) return NotFound("Transporteur profile not found");

        _userService.UpdateTransporteurAvailability(transporteur.Id, dto.IsAvailable);
        return Ok(new { message = "Availability updated", isAvailable = dto.IsAvailable });
    }
}
