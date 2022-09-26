namespace KolmeoCodingTest.Application.Errors;

public class EntityNotFoundException : Exception
{
    public string EntityName { get; }
    public int EntityId { get; }

    public EntityNotFoundException(string entityName, int entityId) : base($"{entityName} with ID {entityId} could not be found")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}