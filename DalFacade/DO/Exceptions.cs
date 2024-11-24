namespace DO;

// Exception thrown when attempting to access a non-existent entity in the data layer
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

// Exception thrown when attempting to create an entity that already exists in the data layer
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}

// Exception thrown when an entity cannot be deleted due to certain constraints
public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}
