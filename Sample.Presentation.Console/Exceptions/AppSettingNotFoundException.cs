using System;
using System.Runtime.Serialization;

namespace Sample.Presentation.Console
{
    [Serializable]
    public class AppSettingNotFoundException : Exception
    {
        public AppSettingNotFoundException()
        {
        }

        public AppSettingNotFoundException(string message) : base(message)
        {
        }

        public AppSettingNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AppSettingNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}