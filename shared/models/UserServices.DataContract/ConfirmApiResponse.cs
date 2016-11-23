//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The link account response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    ///     The link account response.
    /// </summary>
    public class ConfirmApiResponse
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the activity id.
        /// </summary>
        [JsonProperty(PropertyName = "activity_id")]
        public Guid ActivityId { get; set; }

        /// <summary>
        ///     Gets or sets the code.
        /// </summary>
        [JsonProperty(PropertyName = "result_code")]
        public string Code { get; set; }

        /// <summary>
        ///     Gets or sets the explanation.
        /// </summary>
        [JsonProperty(PropertyName = "explanation")]
        public string Explanation { get; set; }

        #endregion
    }
}