namespace keycloack_project.Models;

/// <summary>
/// Base user profile - common fields stored in Keycloak.
/// Keycloak stores: FirstName, LastName, Email, Username.
/// Role-specific fields are stored in the application database.
/// </summary>
public class UserProfile
{
    public Guid Id { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Admin-specific profile fields (extends base user from Keycloak).
/// </summary>
public class AdminProfile : UserProfile
{
    public string Department { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public bool CanManageUsers { get; set; } = true;
    public bool CanViewReports { get; set; } = true;
    public bool CanManageSettings { get; set; } = true;
}

/// <summary>
/// Gerant (Manager) specific profile fields.
/// </summary>
public class GerantProfile : UserProfile
{
    public string Zone { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int MaxTransporteursManaged { get; set; } = 10;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Transporteur (Driver/Transporter) specific profile fields.
/// </summary>
public class TransporteurProfile : UserProfile
{
    public string LicenseNumber { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string VehiclePlate { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public Guid? AssignedGerantId { get; set; }
}
