namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
    public BlNullPropertyException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidPropertyException : Exception
{
    public BlInvalidPropertyException(string? message) : base(message) { }
    public BlInvalidPropertyException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlOperationFailedException : Exception
{
    public BlOperationFailedException(string? message) : base(message) { }
    public BlOperationFailedException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlAccessDeniedException : Exception
{
    public BlAccessDeniedException(string? message) : base(message) { }
    public BlAccessDeniedException(string message, Exception innerException)
        : base(message, innerException) { }
}
[Serializable]
public class BlIncorrectPasswordException : Exception
{
    // Constructor with only a message
    public BlIncorrectPasswordException(string? message) : base(message) { }

    // Constructor with a message and an inner exception for more context
    public BlIncorrectPasswordException(string message, Exception innerException)
        : base(message, innerException) { }

    // Constructor for serialization (used when exceptions are transferred across application domains)
    protected BlIncorrectPasswordException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}

[Serializable]
public class BlWrongItemtException : Exception
{
    // Constructor with only a message
    public BlWrongItemtException(string? message) : base(message) { }

    // Constructor with a message and an inner exception for more context
    public BlWrongItemtException(string message, Exception innerException)
        : base(message, innerException) { }

    // Constructor for serialization (used when exceptions are transferred across application domains)
    protected BlWrongItemtException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}