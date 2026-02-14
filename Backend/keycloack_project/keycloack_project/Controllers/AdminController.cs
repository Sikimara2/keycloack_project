using keycloack_project.Authorization;
using keycloack_project.DTOs;
using keycloack_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keycloack_project.Controllers;

/// <summary>
/// Admin-only endpoints.
/// Only users with the "admin" role in Keycloak can access these.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class AdminController : ControllerBase
{
    private readonly UserService _userService;

    public AdminController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// GET /api/admin/dashboard - Admin dashboard with overall stats.
    /// </summary>
    [HttpGet("dashboard")]
    public ActionResult<AdminDashboardDto> GetDashboard()
    {
        return Ok(_userService.GetAdminDashboard());
    }

    /// <summary>
    /// GET /api/admin/users - List all users (all roles).
    /// </summary>
    [HttpGet("users")]
    public ActionResult<List<UserProfileDto>> GetAllUsers()
    {
        var users = new List<UserProfileDto>();

        users.AddRange(_userService.GetAllAdmins().Select(a => new AdminProfileDto
        {
            Id = a.Id, FirstName = a.FirstName, LastName = a.LastName,
            Email = a.Email, Role = a.Role, CreatedAt = a.CreatedAt,
            Department = a.Department, EmployeeId = a.EmployeeId,
            CanManageUsers = a.CanManageUsers, CanViewReports = a.CanViewReports,
            CanManageSettings = a.CanManageSettings
        }));

        users.AddRange(_userService.GetAllGerants().Select(g => new GerantProfileDto
        {
            Id = g.Id, FirstName = g.FirstName, LastName = g.LastName,
            Email = g.Email, Role = g.Role, CreatedAt = g.CreatedAt,
            Zone = g.Zone, PhoneNumber = g.PhoneNumber,
            MaxTransporteursManaged = g.MaxTransporteursManaged, IsActive = g.IsActive
        }));

        users.AddRange(_userService.GetAllTransporteurs().Select(t => new TransporteurProfileDto
        {
            Id = t.Id, FirstName = t.FirstName, LastName = t.LastName,
            Email = t.Email, Role = t.Role, CreatedAt = t.CreatedAt,
            LicenseNumber = t.LicenseNumber, VehicleType = t.VehicleType,
            VehiclePlate = t.VehiclePlate, PhoneNumber = t.PhoneNumber,
            IsAvailable = t.IsAvailable, AssignedGerantId = t.AssignedGerantId
        }));

        return Ok(users);
    }

    /// <summary>
    /// POST /api/admin/assign-transporteur - Assign a transporteur to a gerant.
    /// </summary>
    [HttpPost("assign-transporteur")]
    public ActionResult AssignTransporteur([FromBody] AssignTransporteurDto dto)
    {
        var success = _userService.AssignTransporteurToGerant(dto.TransporteurId, dto.GerantId);
        if (!success)
            return NotFound("Transporteur not found");
        return Ok(new { message = "Transporteur assigned successfully" });
    }

    /// <summary>
    /// GET /api/admin/gerants - List all gerants.
    /// </summary>
    [HttpGet("gerants")]
    public ActionResult<List<GerantProfileDto>> GetAllGerants()
    {
        var gerants = _userService.GetAllGerants().Select(g => new GerantProfileDto
        {
            Id = g.Id, FirstName = g.FirstName, LastName = g.LastName,
            Email = g.Email, Role = g.Role, CreatedAt = g.CreatedAt,
            Zone = g.Zone, PhoneNumber = g.PhoneNumber,
            MaxTransporteursManaged = g.MaxTransporteursManaged, IsActive = g.IsActive
        }).ToList();
        return Ok(gerants);
    }

    /// <summary>
    /// GET /api/admin/transporteurs - List all transporteurs.
    /// </summary>
    [HttpGet("transporteurs")]
    public ActionResult<List<TransporteurProfileDto>> GetAllTransporteurs()
    {
        var transporteurs = _userService.GetAllTransporteurs().Select(t => new TransporteurProfileDto
        {
            Id = t.Id, FirstName = t.FirstName, LastName = t.LastName,
            Email = t.Email, Role = t.Role, CreatedAt = t.CreatedAt,
            LicenseNumber = t.LicenseNumber, VehicleType = t.VehicleType,
            VehiclePlate = t.VehiclePlate, PhoneNumber = t.PhoneNumber,
            IsAvailable = t.IsAvailable, AssignedGerantId = t.AssignedGerantId
        }).ToList();
        return Ok(transporteurs);
    }
}
