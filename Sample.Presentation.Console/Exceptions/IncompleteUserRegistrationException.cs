using System;
using System.Runtime.Serialization;

namespace Sample.Presentation.Console
{
    [Serializable]
    public class IncompleteUserRegistrationException : Exception
    {
        public IncompleteUserRegistrationException()
        {
        }

        public IncompleteUserRegistrationException(string message) : base(message)
        {
        }

        public IncompleteUserRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IncompleteUserRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}