using System;

namespace Timeline.Exceptions
{
    internal class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(Type classType, string methodName, Type parameterType)
            : base($"This class ({classType.FullName}) has no method named \"{methodName}\" that takes this parameter ({parameterType}).")
        {
        }
    }
}