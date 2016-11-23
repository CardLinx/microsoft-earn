//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The unity dependency resolver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd
{
    using System.Web.Http.Dependencies;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// The unity dependency resolver.
    /// </summary>
    public class UnityDependencyResolver : UnityDependencyScope, IDependencyResolver
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public UnityDependencyResolver(IUnityContainer container)
            : base(container)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The begin scope.
        /// </summary>
        /// <returns>
        /// The <see cref="IDependencyScope"/>.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            IUnityContainer childContainer = this.Container.CreateChildContainer();

            return new UnityDependencyScope(childContainer);
        }

        #endregion
    }
}