using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AltinnCLI.Core.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for finding type that implements specific Interface in an Assembly
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Get all Types that implments type T in the assembly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static List<Type> GetTypesAssignableFrom<T>(this Assembly assembly)
        {
            return assembly.GetTypesAssignableFrom(typeof(T));
        }

        /// <summary>
        /// Get all Types that implements the compareType
        /// </summary>
        /// <param name="assembly">Assembly to search for types</param>
        /// <param name="compareType">Type to search for</param>
        /// <returns>List of types that is of requested type</returns>
        public static List<Type> GetTypesAssignableFrom(this Assembly assembly, Type compareType)
        {
            List<Type> ret = new List<Type>();
            foreach (var type in assembly.DefinedTypes)
            {
                if (compareType.IsAssignableFrom(type) && compareType != type)
                {
                    ret.Add(type);
                }
            }
            return ret;
        }
    }
}