using System;

namespace Timeline.Exceptions
{
    internal class MissingDefaultConstructorException : Exception
    {
        public MissingDefaultConstructorException(Type type)
            : base($"This class has no default constructor ({type.FullName}).")
        {
        }
    }
}