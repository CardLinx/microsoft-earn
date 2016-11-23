//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The Merchant Information
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The Merchant Information
    /// </summary>
    [DataContract]
    public class MerchantInformation
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the subscription status
        /// </summary>
        [DataMember(Name = "is_active")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the subscription type
        /// </summary>
        [DataMember(Name = "subscription_type")]
        public string SubscriptionType { get; set; }

        /// <summary>
        /// Gets or sets the merchant preferences
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "preferences")]
        public MerchantPreferences Preferences { get; set; }

        #endregion
    }
}