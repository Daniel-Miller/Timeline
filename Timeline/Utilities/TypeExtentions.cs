using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Utilities
{
    public static class TypeExtentions
    {
        /// <summary>
        /// Returns the assembly-qualified class name without the version, culture, and public key token.
        /// </summary>
        public static string GetClassName(this Type type)
        {
            return $"{type.FullName}, {System.Reflection.Assembly.GetAssembly(type).GetName().Name}";
        }
    }
}
