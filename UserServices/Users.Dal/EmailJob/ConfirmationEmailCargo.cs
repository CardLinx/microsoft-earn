//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Cargo for sending email confirmation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using Newtonsoft.Json;

    /// <summary>
    ///     Cargo for sending email confirmation
    /// </summary>
    public class ConfirmationEmailCargo : PriorityEmailCargo
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the entity type.
        /// </summary>
        [JsonProperty(PropertyName = "entity_type")]
        public EntityType EntityType { get; set; }

        /// <summary>
        ///     Gets or sets the user id hash.
        /// </summary>
        [JsonProperty(PropertyName = "user_id_hash")]
        public string UserIdHash { get; set; }

        /// <summary>
        ///     Gets or sets the confirmation code.
        /// </summary>
        [JsonProperty(PropertyName = "confirmation_code")]
        public int ConfirmationCode { get; set; }

        #endregion

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Activity Id: {0}; UserIdHash: {1}; EntityType: {2}",
               this.Id, this.UserIdHash, this.EntityType);
        }
    }
}