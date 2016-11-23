//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The Merchant Preferences
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The Merchant Preferences
    /// </summary>
    [DataContract]
    public class MerchantPreferences
    {
        #region Public Properties

        /// <summary>
        /// Interval for receiving email
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "email_report_interval")]
        public string EmailReportInterval { get; set; }

        #endregion
    }
}