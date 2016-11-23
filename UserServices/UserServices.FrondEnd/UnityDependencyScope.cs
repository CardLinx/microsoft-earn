//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The unity dependency scope.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// The unity dependency scope.
    /// </summary>
    public class UnityDependencyScope : IDependencyScope
    {
        #region Fields

        /// <summary>
        /// The Container.
        /// </summary>
        protected readonly IUnityContainer Container;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyScope"/> class.
        /// </summary>
        /// <param name="container">
        /// The Container.
        /// </param>
        /// <exception cref="ArgumentNullException"> The Container is null </exception>
        public UnityDependencyScope(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.Container = container;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Container.Dispose();
        }

        /// <summary>
        /// The get service.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The service
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (this.Container.IsRegistered(serviceType))
            {
                return this.Container.Resolve(serviceType);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The get services.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The services
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (this.Container.IsRegistered(serviceType))
            {
                return this.Container.ResolveAll(serviceType);
            }
            else
            {
                return new List<object>();
            }
        }

        #endregion
    }
}