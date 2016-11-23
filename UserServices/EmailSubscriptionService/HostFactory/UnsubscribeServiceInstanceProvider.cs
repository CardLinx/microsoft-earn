//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The lookup service instance provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.EmailSubscription.Service.HostFactory
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    using LoMo.UserServices.Storage.HCP;

    /// <summary>
    /// The unsubscribe service instance provider.
    /// </summary>
    public class UnsubscribeServiceInstanceProvider : IInstanceProvider, IContractBehavior
    {
        #region Fields

        /// <summary>
        ///     The unsubscribe service settings.
        /// </summary>
        private readonly UnsubscribeServiceSettings unsubscribeServiceSettings;

        /// <summary>
        ///     The hcp commands queue
        /// </summary>
        private readonly IHcpCommandsQueue hcpCommandsQueue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeServiceInstanceProvider"/> class.
        /// </summary>
        /// <param name="hcpCommandsQueue">
        /// The hcp commands queue
        /// </param>
        /// <param name="serviceSettings">
        /// The unsubscribe service settings.
        /// </param>
        public UnsubscribeServiceInstanceProvider(IHcpCommandsQueue hcpCommandsQueue, UnsubscribeServiceSettings serviceSettings)
        {
            if (hcpCommandsQueue == null)
            {
                throw new ArgumentNullException("hcpCommandsQueue");
            }

            if (serviceSettings == null)
            {
                throw new ArgumentNullException("serviceSettings");
            }

            this.unsubscribeServiceSettings = serviceSettings;
            this.hcpCommandsQueue = hcpCommandsQueue;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add binding parameters.
        /// </summary>
        /// <param name="contractDescription">
        /// The contract description.
        /// </param>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="bindingParameters">
        /// The binding parameters.
        /// </param>
        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// The apply client behavior.
        /// </summary>
        /// <param name="contractDescription">
        /// The contract description.
        /// </param>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="clientRuntime">
        /// The client runtime.
        /// </param>
        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        /// <summary>
        /// The apply dispatch behavior.
        /// </summary>
        /// <param name="contractDescription">
        /// The contract description.
        /// </param>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="dispatchRuntime">
        /// The dispatch runtime.
        /// </param>
        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceProvider = this;
        }

        /// <summary>
        /// The get instance.
        /// </summary>
        /// <param name="instanceContext">
        /// The instance context.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return this.GetInstance(instanceContext);
        }

        /// <summary>
        /// The get instance.
        /// </summary>
        /// <param name="instanceContext">
        /// The instance context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetInstance(InstanceContext instanceContext)
        {
            return new UnsubscribeService(this.hcpCommandsQueue, this.unsubscribeServiceSettings);
        }

        /// <summary>
        /// The release instance.
        /// </summary>
        /// <param name="instanceContext">
        /// The instance context.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="contractDescription">
        /// The contract description.
        /// </param>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}