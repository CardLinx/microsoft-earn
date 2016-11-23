//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The link account request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using Newtonsoft.Json;

    /// <summary>
    ///     The link account request.
    /// </summary>
    public class LinkAccountRequest
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the code.
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        /// <summary>
        ///     Gets or sets the activity id.
        /// </summary>
        [JsonProperty(PropertyName = "uh")]
        public string UserIdHash { get; set; }

        #endregion
    }
}