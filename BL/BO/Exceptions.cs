using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class BLException : Exception
    {
        // Default constructor.
        public BLException() { }

        // Constructor accepting a message explaining the exception.
        public BLException(string? message) : base(message) { }

        // Constructor that also accepts an inner exception to chain exceptions.
        public BLException(string message, Exception innerException)
                : base(message, innerException) { }

        // Constructor for serialization support.
        protected BLException(SerializationInfo info, StreamingContext context)
                : base(info, context) { }
    }
    // Exception class for errors related to loading or creating XML files in the business logic.
    [Serializable]
    public class BlXMLFileLoadCreateException : Exception
    {
        // Constructor accepting a message explaining the exception.
        public BlXMLFileLoadCreateException(string? message) : base(message) { }

        // Constructor that also accepts an inner exception to chain exceptions.
        public BlXMLFileLoadCreateException(string message, Exception innerException)
                : base(message, innerException) { }
    }

    // Exception class for errors where a requested object does not exist in the business logic.
    [Serializable]
    public class BlDoesNotExistException : Exception
    {
        public BlDoesNotExistException(string? message) : base(message) { }

        // Constructor allowing the exception to carry an inner exception, useful for debugging.
        public BlDoesNotExistException(string message, Exception innerException)
                    : base(message, innerException) { }
    }

    // Exception class for errors where an item is incorrect or invalid in the business logic.
    [Serializable]
    public class BlWrongItemException : Exception
    {
        public BlWrongItemException(string? message) : base(message) { }

        // Constructor for providing additional context with an inner exception.
        public BlWrongItemException(string message, Exception innerException)
                    : base(message, innerException) { }
    }

    // Exception class for situations where a delete operation cannot be completed.
    [Serializable]
    public class BlDeleteNotPossibleException : Exception
    {
        public BlDeleteNotPossibleException(string? message) : base(message) { }

        // Constructor that allows chaining of exceptions for more detailed error handling.
        public BlDeleteNotPossibleException(string message, Exception innerException)
                : base(message, innerException) { }
    }

    // Exception class for situations where an object already exists in the business logic.
    [Serializable]
    public class BlAlreadyExistsException : Exception
    {
        public BlAlreadyExistsException(string? message) : base(message) { }

        // Constructor for passing an inner exception along with the message.
        public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
    }

    // Exception class for null property errors in the business logic.
    public class BlNullPropertyException : Exception
    {
        // Constructor accepting only the message for this exception type.
        public BlNullPropertyException(string? message) : base(message) { }
    }

    // Exception class for errors due to invalid or wrong input in the business logic.
    public class BlWrongInputException : Exception
    {
        // Constructor with the message for the exception.
        public BlWrongInputException(string? message) : base(message) { }

        // Constructor that accepts both a message and an inner exception.
        public BlWrongInputException(string message, Exception innerException)
                : base(message, innerException) { }
    }
    public class BLTemporaryNotAvailableException : Exception
    {
        // Constructor that takes a message describing the exception.
        public BLTemporaryNotAvailableException(string? message) : base(message) { }

        // Constructor that takes a message and an inner exception.
        public BLTemporaryNotAvailableException(string message, Exception innerException)
                : base(message, innerException) { }
    }


}
