namespace Speakeasy.Server.Models.Exceptions;

public class GroupNotFoundException : Exception
{
    public GroupNotFoundException(Guid? id = null) :
        base($"Group not found by {id}")
    {
        Id = id;
    }
    
    public Guid? Id { get; private set; }
}