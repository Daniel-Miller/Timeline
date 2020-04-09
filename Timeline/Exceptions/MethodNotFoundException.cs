using System;
using System.Runtime.Serialization;

namespace Timeline.Exceptions
{
    [Serializable]
    internal class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(Type classType, string methodName, Type parameterType)
            : base($"This class ({classType.FullName}) has no method named \"{methodName}\" that takes this parameter ({parameterType}).")
        {
        }

        protected MethodNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}