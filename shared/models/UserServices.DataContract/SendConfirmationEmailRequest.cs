//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The send confirmation email request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using Newtonsoft.Json;

    /// <summary>
    ///     The send confirmation email request.
    /// </summary>
    public class SendConfirmationEmailRequest
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the confirmation type.
        /// </summary>
        [JsonProperty(PropertyName = "confirmation_type")]
        public string ConfirmationType { get; set; }

        /// <summary>
        ///     Gets or sets the user id hash.
        /// </summary>
        [JsonProperty(PropertyName = "uh")]
        public string UserIdHash { get; set; }

        #endregion
    }
}