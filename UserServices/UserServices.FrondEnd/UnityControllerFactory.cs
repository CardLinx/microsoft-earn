//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The unity controller factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.SessionState;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// The unity controller factory.
    /// </summary>
    public class UnityControllerFactory : IControllerFactory
    {
        #region Fields

        /// <summary>
        /// The container.
        /// </summary>
        private readonly IUnityContainer container;

        /// <summary>
        /// The inner factory.
        /// </summary>
        private readonly IControllerFactory innerFactory;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityControllerFactory"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public UnityControllerFactory(IUnityContainer container)
            : this(container, new DefaultControllerFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityControllerFactory"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="innerFactory">
        /// The inner factory.
        /// </param>
        protected UnityControllerFactory(IUnityContainer container, IControllerFactory innerFactory)
        {
            this.container = container;
            this.innerFactory = innerFactory;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create controller.
        /// </summary>
        /// <param name="requestContext">
        /// The request context.
        /// </param>
        /// <param name="controllerName">
        /// The controller name.
        /// </param>
        /// <returns>
        /// The <see cref="IController"/>.
        /// </returns>
        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            controllerName = controllerName == null ? null : controllerName.ToLowerInvariant();
            if (this.container.IsRegistered<IController>(controllerName))
            {
                return this.container.Resolve<IController>(controllerName);
            }
            else
            {
                return this.innerFactory.CreateController(requestContext, controllerName);
            }
        }

        /// <summary>
        /// The get controller session behavior.
        /// </summary>
        /// <param name="requestContext">
        /// The request context.
        /// </param>
        /// <param name="controllerName">
        /// The controller name.
        /// </param>
        /// <returns>
        /// The <see cref="SessionStateBehavior"/>.
        /// </returns>
        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }

        /// <summary>
        /// The release controller.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public void ReleaseController(IController controller)
        {
            this.container.Teardown(controller);
        }

        #endregion
    }
}