//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Cargo base class for Priority email jobs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    ///  Cargo base class for Priority email jobs
    /// </summary>
    public class PriorityEmailCargo
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the email address.
        /// </summary>
        [JsonProperty(PropertyName = "email_address")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Activity Id: {0}", this.Id);
        }
    }
}