//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the EmailJobHints type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.UserServices.DealsMailing
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The email job hints.
    /// </summary>
    [DataContract]
    public class EmailJobHints
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether this is a test or real email job
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "is_test")]
        public bool IsTest { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate whether to include deals for the email job. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "include_deals")]
        public bool IncludeDeals { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate whether this is a test email (email to verify if everything looks good before launching a campaign)
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "is_test_email")]
        public bool IsTestEmail { get; set; }

        #endregion
    }
}