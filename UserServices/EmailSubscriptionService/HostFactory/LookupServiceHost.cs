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

    using Users.Dal;

    /// <summary>
    ///     The lookup service host.
    /// </summary>
    public class LookupServiceHost : ServiceHost
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupServiceHost"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        /// <param name="lookupServiceSettings">
        /// The lookup service settings.
        /// </param>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="baseAddresses">
        /// The base addresses.
        /// </param>
        public LookupServiceHost(IUsersDal usersDal, LookupServiceSettings lookupServiceSettings, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            if (usersDal == null)
            {
                throw new ArgumentNullException("usersDal");
            }

            if (lookupServiceSettings == null)
            {
                throw new ArgumentNullException("lookupServiceSettings");
            }

            foreach (ContractDescription cd in this.ImplementedContracts.Values)
            {
                cd.Behaviors.Add(new LookupServiceInstanceProvider(usersDal, lookupServiceSettings));
            }

            this.Description.Behaviors.Find<ServiceBehaviorAttribute>().InstanceContextMode = InstanceContextMode.Single;
        }

        #endregion
    }
}