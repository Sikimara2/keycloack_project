namespace keycloack_project.DTOs;

// --- Registration DTOs ---

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
