namespace Speakeasy.Server.Models.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string? id = null, string? userName = null) :
        base($"User not found by {id} and userName {userName}")
    {
        Id = id;
        UserName = userName;
    }
    
    public string? Id { get; set; }
    
    public string? UserName { get; set; }
}