using System;
using System.Runtime.Serialization;

namespace Sample.Presentation.Console
{
    [Serializable]
    public class ConnectionStringNotFoundException : Exception
    {
        public ConnectionStringNotFoundException()
        {
        }

        public ConnectionStringNotFoundException(string message) : base(message)
        {
        }

        public ConnectionStringNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConnectionStringNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}