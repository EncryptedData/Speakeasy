namespace Speakeasy.Server.Models.Exceptions;

public class RoleNotFoundException : Exception
{
    public RoleNotFoundException(Guid roleId) :
        base($"Role by name {roleId} not found.")
    {
        RoleId = roleId;
    }
    
    public RoleNotFoundException(string roleName) :
        base($"Role by name {roleName} not found.")
    {
        RoleName = roleName;
    }
    
    public Guid? RoleId { get; private set; }
    public string? RoleName { get; set; }
}