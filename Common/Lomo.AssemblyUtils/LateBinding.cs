//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Utility methods to late bind assemblies and create objects for classes defined within. Primarily for use in
//    mocking external components during unit and component testing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.AssemblyUtils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Contains methods to late bind assemblies and create objects for classes defined within.
    /// </summary>
    public static class LateBinding
    {
        /// <summary>
        /// Gets the Types that exist within the specified assembly.
        /// </summary>
        /// <param name="assemblyName">
        /// The name of the assembly from which to load types.
        /// </param>
        /// <returns>
        /// The list of types from the specified assembly.
        /// </returns>
        /// <remarks>
        /// Assembly from which to load types must be in the same folder as the currently executing assembly.
        /// </remarks>
        public static IEnumerable<Type> GetLateBoundAssemblyTypes(string assemblyName)
        {
            List<Type> result = new List<Type>();
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (string.IsNullOrWhiteSpace(directoryName) == false)
            {
                string assemblyPath = new Uri(directoryName).LocalPath;
                Assembly assembly = Assembly.LoadFrom(string.Concat(assemblyPath, "\\", assemblyName));
                result.AddRange(assembly.GetTypes());
            }

            return result;
        }

        /// <summary>
        /// Creates an instance of the type with the specified name from the specified list of types.
        /// </summary>
        /// <typeparam name="TObjectType">
        /// The type of the object to build.
        /// </typeparam>
        /// <param name="className">
        /// The name of the class of which an instance will be built.
        /// </param>
        /// <param name="lateBoundAssemblyTypes">
        /// The list of types from which to find the class to be built.
        /// </param>
        /// <returns>
        /// The instance of the class.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter lateBoundAssemblyTypes cannot be null.
        /// </exception>
        public static TObjectType BuildObjectFromLateBoundAssembly<TObjectType>(
                                                                              string className,
                                                                              IEnumerable<Type> lateBoundAssemblyTypes)
        {
            if (lateBoundAssemblyTypes == null)
            {
                throw new ArgumentNullException(
                                                "lateBoundAssemblyTypes",
                                                "Parameter lateBoundAssemblyTypes cannot be null.");
            }

            TObjectType result = default(TObjectType);
            foreach (Type type in lateBoundAssemblyTypes)
            {
                if (type.Name == className)
                {
                    result = (TObjectType)Activator.CreateInstance(type);
                    break;
                }
            }

            return result;
        }
    }
}