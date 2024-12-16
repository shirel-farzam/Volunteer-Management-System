using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class BlXMLFileLoadCreateException : Exception
    {
        public BlXMLFileLoadCreateException(string? message) : base(message) { }
        public BlXMLFileLoadCreateException(string message, Exception innerException)
                : base(message, innerException) { }
    }

    [Serializable]
    public class BlDoesNotExistException : Exception
    {
        public BlDoesNotExistException(string? message) : base(message) { }
        public BlDoesNotExistException(string message, Exception innerException)
                    : base(message, innerException) { }
    }

    [Serializable]
    public class BlWrongItemException : Exception
    {
        public BlWrongItemException(string? message) : base(message) { }
        public BlWrongItemException(string message, Exception innerException)
                    : base(message, innerException) { }
    }

    [Serializable]
    public class BlDeleteNotPossibleException : Exception
    {
        public BlDeleteNotPossibleException(string? message) : base(message) { }
        public BlDeleteNotPossibleException(string message, Exception innerException)
                : base(message, innerException) { }
    }

    [Serializable]
    public class BlAlreadyExistsException : Exception
    {
        public BlAlreadyExistsException(string? message) : base(message) { }
        public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
    }

    public class BlNullPropertyException : Exception
    {
        public BlNullPropertyException(string? message) : base(message) { }
    }

    public class BlWrongInputException : Exception
    {
        public BlWrongInputException(string? message) : base(message) { }
        public BlWrongInputException(string message, Exception innerException)
                : base(message, innerException) { }
    }

}
