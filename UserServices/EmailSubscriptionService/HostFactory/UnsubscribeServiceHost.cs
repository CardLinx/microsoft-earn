//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The lookup service host.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.EmailSubscription.Service.HostFactory
{
    using System;

    using System.ServiceModel;
    using System.ServiceModel.Description;

    using LoMo.UserServices.Storage.HCP;

    using Users.Dal;

    /// <summary>
    /// The unsubscribe service host.
    /// </summary>
    public class UnsubscribeServiceHost : ServiceHost
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeServiceHost"/> class.
        /// </summary>
        /// <param name="hcpCommandsQueue">
        /// The hcp commands queue
        /// </param>
        /// <param name="serviceSettings">
        /// The unsubscribe service settings.
        /// </param>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="baseAddresses">
        /// The base addresses.
        /// </param>
        public UnsubscribeServiceHost(IHcpCommandsQueue hcpCommandsQueue, UnsubscribeServiceSettings serviceSettings, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            if (hcpCommandsQueue == null)
            {
                throw new ArgumentNullException("hcpCommandsQueue");
            }

            if (serviceSettings == null)
            {
                throw new ArgumentNullException("serviceSettings");
            }

            foreach (ContractDescription cd in this.ImplementedContracts.Values)
            {
                cd.Behaviors.Add(new UnsubscribeServiceInstanceProvider(hcpCommandsQueue, serviceSettings));
            }
        }

        #endregion
    }
}