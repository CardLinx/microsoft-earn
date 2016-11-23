//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The user contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Analytics.API.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The user contract.
    /// </summary>
    public class UserContract
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember(Name = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the first card added date time.
        /// </summary>
        [DataMember(Name = "clo_first_card_added_datetime")]
        public DateTime? CloFirstCardAddedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the user added date time.
        /// </summary>
        [DataMember(Name = "clo_user_added_datetime")]
        public DateTime? CloUserAddedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the redemptions.
        /// </summary>
        [DataMember(Name = "redemtpions")]
        public IList<RedemptionContract> Redemptions { get; set; }
        
        #endregion
    }
}