namespace keycloack_project.Authorization;

/// <summary>
/// Constants for Keycloak role names used throughout the application.
/// These roles must match the roles configured in Keycloak realm.
/// </summary>
public static class KeycloakRoles
{
    public const string Admin = "admin";
    public const string Gerant = "gerant";
    public const string Transporteur = "transporteur";
}

/// <summary>
/// Policy names for authorization.
/// Policies can combine multiple roles for flexible access control.
/// </summary>
public static class AuthPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string GerantOnly = "GerantOnly";
    public const string TransporteurOnly = "TransporteurOnly";
    public const string AdminOrGerant = "AdminOrGerant";
    public const string AllAuthenticated = "AllAuthenticated";
}
