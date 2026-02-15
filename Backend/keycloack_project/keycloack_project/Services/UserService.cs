using keycloack_project.DTOs;
using keycloack_project.Models;

namespace keycloack_project.Services;

/// <summary>
/// In-memory user service for demonstration.
/// In production, replace with a real database (EF Core + PostgreSQL, etc.).
/// Common fields (FirstName, LastName, Email) come from Keycloak token.
/// Role-specific fields are stored/managed here.
/// </summary>
public class UserService
{
    // In-memory storage (replace with DB in production)
    private readonly List<AdminProfile> _admins = new();
    private readonly List<GerantProfile> _gerants = new();
    private readonly List<TransporteurProfile> _transporteurs = new();

    // --- Admin operations ---

    public AdminProfile CreateAdmin(string keycloakUserId, string firstName, string lastName,
        string email, RegisterAdminDto dto)
    {
        var admin = new AdminProfile
        {
            Id = Guid.NewGuid(),
            KeycloakUserId = keycloakUserId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = "admin",
            Department = dto.Department,
            EmployeeId = dto.EmployeeId
        };
        _admins.Add(admin);
        return admin;
    }

    public AdminProfile CreateAdminFromFullRegistration(string keycloakUserId, RegisterAdminRequestDto dto)
    {
        var admin = new AdminProfile
        {
            Id = Guid.NewGuid(),
            KeycloakUserId = keycloakUserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = "admin",
            Department = dto.Department,
            EmployeeId = dto.EmployeeId,
            PhoneNumber = dto.PhoneNumber,
            JobTitle = dto.JobTitle,
            OfficeLocation = dto.OfficeLocation,
            DirectLine = dto.DirectLine,
            HireDate = dto.HireDate,
            ManagerName = dto.ManagerName,
            CanManageUsers = dto.CanManageUsers,
            CanViewReports = dto.CanViewReports,
            CanManageSettings = dto.CanManageSettings,
            CanApproveExpenses = dto.CanApproveExpenses
        };
        _admins.Add(admin);
        return admin;
    }

    public List<AdminProfile> GetAllAdmins() => _admins.ToList();

    public AdminProfile? GetAdminByKeycloakId(string keycloakUserId) =>
        _admins.FirstOrDefault(a => a.KeycloakUserId == keycloakUserId);

    // --- Gerant operations ---

    public GerantProfile CreateGerant(string keycloakUserId, string firstName, string lastName,
        string email, RegisterGerantDto dto)
    {
        var gerant = new GerantProfile
        {
            Id = Guid.NewGuid(),
            KeycloakUserId = keycloakUserId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = "gerant",
            Zone = dto.Zone,
            PhoneNumber = dto.PhoneNumber,
            MaxTransporteursManaged = dto.MaxTransporteursManaged
        };
        _gerants.Add(gerant);
        return gerant;
    }

    public GerantProfile CreateGerantFromFullRegistration(string keycloakUserId, RegisterGerantRequestDto dto)
    {
        var gerant = new GerantProfile
        {
            Id = Guid.NewGuid(),
            KeycloakUserId = keycloakUserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = "gerant",
            Zone = dto.Zone,
            PhoneNumber = dto.PhoneNumber,
            MobileNumber = dto.MobileNumber,
            MaxTransporteursManaged = dto.MaxTransporteursManaged,
            Address = dto.Address,
            City = dto.City,
            PostalCode = dto.PostalCode,
            EmergencyContact = dto.EmergencyContact,
            EmergencyPhone = dto.EmergencyPhone,
            DateOfBirth = dto.DateOfBirth,
            Nationality = dto.Nationality,
            LanguagesSpoken = dto.LanguagesSpoken,
            YearsOfExperience = dto.YearsOfExperience,
            PreviousEmployer = dto.PreviousEmployer
        };
        _gerants.Add(gerant);
        return gerant;
    }

    public List<GerantProfile> GetAllGerants() => _gerants.ToList();

    public GerantProfile? GetGerantByKeycloakId(string keycloakUserId) =>
        _gerants.FirstOrDefault(g => g.KeycloakUserId == keycloakUserId);

    public GerantProfile? GetGerantById(Guid id) =>
        _gerants.FirstOrDefault(g => g.Id == id);

    // --- Transporteur operations ---

    public TransporteurProfile CreateTransporteur(string keycloakUserId, string firstName,
        string lastName, string email, RegisterTransporteurDto dto)
    {
        var transporteur = new TransporteurProfile
        {
            Id = Guid.NewGuid(),
            KeycloakUserId = keycloakUserId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = "transporteur",
            LicenseNumber = dto.LicenseNumber,
            VehicleType = dto.VehicleType,
            VehiclePlate = dto.VehiclePlate,
            PhoneNumber = dto.PhoneNumber,
            AssignedGerantId = dto.AssignedGerantId
        };
        _transporteurs.Add(transporteur);
        return transporteur;
    }

    public TransporteurProfile CreateTransporteurFromFullRegistration(string keycloakUserId, RegisterTransporteurRequestDto dto)
    {
        var transporteur = new TransporteurProfile
        {
            Id = Guid.NewGuid(),
            KeycloakUserId = keycloakUserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = "transporteur",
            LicenseNumber = dto.LicenseNumber,
            VehicleType = dto.VehicleType,
            VehiclePlate = dto.VehiclePlate,
            PhoneNumber = dto.PhoneNumber,
            MobileNumber = dto.MobileNumber,
            Address = dto.Address,
            City = dto.City,
            PostalCode = dto.PostalCode,
            LicenseExpiryDate = dto.LicenseExpiryDate,
            DateOfBirth = dto.DateOfBirth,
            Nationality = dto.Nationality,
            EmergencyContact = dto.EmergencyContact,
            EmergencyPhone = dto.EmergencyPhone,
            VehicleMake = dto.VehicleMake,
            VehicleModel = dto.VehicleModel,
            VehicleYear = dto.VehicleYear,
            VehicleColor = dto.VehicleColor,
            InsuranceProvider = dto.InsuranceProvider,
            InsurancePolicyNumber = dto.InsurancePolicyNumber,
            InsuranceExpiryDate = dto.InsuranceExpiryDate,
            YearsOfExperience = dto.YearsOfExperience,
            LanguagesSpoken = dto.LanguagesSpoken,
            HasCommercialLicense = dto.HasCommercialLicense,
            HasHazmatCertification = dto.HasHazmatCertification
        };
        _transporteurs.Add(transporteur);
        return transporteur;
    }

    public List<TransporteurProfile> GetAllTransporteurs() => _transporteurs.ToList();

    public TransporteurProfile? GetTransporteurByKeycloakId(string keycloakUserId) =>
        _transporteurs.FirstOrDefault(t => t.KeycloakUserId == keycloakUserId);

    public TransporteurProfile? GetTransporteurById(Guid id) =>
        _transporteurs.FirstOrDefault(t => t.Id == id);

    public List<TransporteurProfile> GetTransporteursByGerant(Guid gerantId) =>
        _transporteurs.Where(t => t.AssignedGerantId == gerantId).ToList();

    public bool UpdateTransporteurAvailability(Guid id, bool isAvailable)
    {
        var t = _transporteurs.FirstOrDefault(x => x.Id == id);
        if (t == null) return false;
        t.IsAvailable = isAvailable;
        return true;
    }

    public bool AssignTransporteurToGerant(Guid transporteurId, Guid gerantId)
    {
        var t = _transporteurs.FirstOrDefault(x => x.Id == transporteurId);
        if (t == null) return false;
        t.AssignedGerantId = gerantId;
        return true;
    }

    // --- Dashboard stats ---

    public AdminDashboardDto GetAdminDashboard() => new()
    {
        TotalUsers = _admins.Count + _gerants.Count + _transporteurs.Count,
        TotalAdmins = _admins.Count,
        TotalGerants = _gerants.Count,
        TotalTransporteurs = _transporteurs.Count,
        ActiveTransporteurs = _transporteurs.Count(t => t.IsAvailable)
    };

    public GerantDashboardDto GetGerantDashboard(Guid gerantId)
    {
        var gerant = _gerants.FirstOrDefault(g => g.Id == gerantId);
        var managed = _transporteurs.Where(t => t.AssignedGerantId == gerantId).ToList();
        return new GerantDashboardDto
        {
            ManagedTransporteurs = managed.Count,
            AvailableTransporteurs = managed.Count(t => t.IsAvailable),
            Zone = gerant?.Zone ?? ""
        };
    }
}
