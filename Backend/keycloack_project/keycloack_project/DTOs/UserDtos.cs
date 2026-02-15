namespace keycloack_project.DTOs;

// =============================================================================
// CUSTOM REGISTRATION DTOs - Used for custom registration flow
// =============================================================================

/// <summary>
/// Complete registration request for Admin role with Keycloak account creation
/// </summary>
public class RegisterAdminRequestDto
{
    // Keycloak user fields
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    // Admin-specific fields
    public required string Department { get; set; }
    public required string EmployeeId { get; set; }
    public required string PhoneNumber { get; set; }
    public required string JobTitle { get; set; }
    public string? OfficeLocation { get; set; }
    public string? DirectLine { get; set; }
    public DateTime? HireDate { get; set; }
    public string? ManagerName { get; set; }
    public bool CanManageUsers { get; set; } = true;
    public bool CanViewReports { get; set; } = true;
    public bool CanManageSettings { get; set; } = true;
    public bool CanApproveExpenses { get; set; } = false;
}

/// <summary>
/// Complete registration request for Gerant (Manager) role
/// </summary>
public class RegisterGerantRequestDto
{
    // Keycloak user fields
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    // Gerant-specific fields
    public required string Zone { get; set; }
    public required string PhoneNumber { get; set; }
    public required string MobileNumber { get; set; }
    public int MaxTransporteursManaged { get; set; } = 10;
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? LanguagesSpoken { get; set; }
    public int YearsOfExperience { get; set; }
    public string? PreviousEmployer { get; set; }
}

/// <summary>
/// Complete registration request for Transporteur (Driver) role
/// </summary>
public class RegisterTransporteurRequestDto
{
    // Keycloak user fields
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    // Transporteur-specific fields
    public required string LicenseNumber { get; set; }
    public required string VehicleType { get; set; }
    public required string VehiclePlate { get; set; }
    public required string PhoneNumber { get; set; }
    public required string MobileNumber { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string? VehicleMake { get; set; }
    public string? VehicleModel { get; set; }
    public int? VehicleYear { get; set; }
    public string? VehicleColor { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime? InsuranceExpiryDate { get; set; }
    public int YearsOfExperience { get; set; }
    public string? LanguagesSpoken { get; set; }
    public bool HasCommercialLicense { get; set; }
    public bool HasHazmatCertification { get; set; }
}

/// <summary>
/// Login request DTO for custom login flow
/// </summary>
public class LoginRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

/// <summary>
/// Login response DTO containing tokens and user info
/// </summary>
public class LoginResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public required string TokenType { get; set; }
    public required UserInfoDto UserInfo { get; set; }
}

/// <summary>
/// User information returned after login
/// </summary>
public class UserInfoDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string[] Roles { get; set; }
    public string? PrimaryRole { get; set; }
}

// =============================================================================
// LEGACY REGISTRATION DTOs - Used for profile completion after Keycloak registration
// =============================================================================

public class RegisterAdminDto
{
    public required string Department { get; set; }
    public required string EmployeeId { get; set; }
}

public class RegisterGerantDto
{
    public required string Zone { get; set; }
    public required string PhoneNumber { get; set; }
    public int MaxTransporteursManaged { get; set; } = 10;
}

public class RegisterTransporteurDto
{
    public required string LicenseNumber { get; set; }
    public required string VehicleType { get; set; }
    public required string VehiclePlate { get; set; }
    public required string PhoneNumber { get; set; }
    public Guid? AssignedGerantId { get; set; }
}

// --- Response DTOs ---

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AdminProfileDto : UserProfileDto
{
    public string Department { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public bool CanManageUsers { get; set; }
    public bool CanViewReports { get; set; }
    public bool CanManageSettings { get; set; }
}

public class GerantProfileDto : UserProfileDto
{
    public string Zone { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int MaxTransporteursManaged { get; set; }
    public bool IsActive { get; set; }
}

public class TransporteurProfileDto : UserProfileDto
{
    public string LicenseNumber { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string VehiclePlate { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public Guid? AssignedGerantId { get; set; }
}

// --- Update DTOs ---

public class UpdateTransporteurAvailabilityDto
{
    public bool IsAvailable { get; set; }
}

public class AssignTransporteurDto
{
    public Guid TransporteurId { get; set; }
    public Guid GerantId { get; set; }
}

// --- Dashboard DTOs ---

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalGerants { get; set; }
    public int TotalTransporteurs { get; set; }
    public int ActiveTransporteurs { get; set; }
}

public class GerantDashboardDto
{
    public int ManagedTransporteurs { get; set; }
    public int AvailableTransporteurs { get; set; }
    public string Zone { get; set; } = string.Empty;
}

public class TransporteurDashboardDto
{
    public bool IsAvailable { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public string AssignedGerantName { get; set; } = string.Empty;
}
