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

    /// <summary>
    /// The unsubscribe service host.
    /// </summary>
    public class SuppressUserServiceHost : ServiceHost
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SuppressUserServiceHost"/> class.
        /// </summary>
        /// <param name="hcpCommandsQueue">
        /// The hcp commands queue
        /// </param>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="baseAddresses">
        /// The base addresses.
        /// </param>
        public SuppressUserServiceHost(IHcpCommandsQueue hcpCommandsQueue, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            if (hcpCommandsQueue == null)
            {
                throw new ArgumentNullException("hcpCommandsQueue");
            }

            foreach (ContractDescription cd in this.ImplementedContracts.Values)
            {
                cd.Behaviors.Add(new SuppressUserInstanceProvider(hcpCommandsQueue));
            }
        }

        #endregion
    }
}