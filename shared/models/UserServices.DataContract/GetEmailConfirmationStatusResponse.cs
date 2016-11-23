//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The get update email status response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The get update email status response.
    /// </summary>
    [DataContract]
    public class GetEmailConfirmationStatusResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether is waiting for confirmation.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "waiting_for_confirmation")]
        public bool WaitingForConfirmation { get; set; }

        /// <summary>
        /// Gets or sets the email to confirm.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "email_to_confirm")]
        public string EmailToConfirm { get; set; }
    }
}