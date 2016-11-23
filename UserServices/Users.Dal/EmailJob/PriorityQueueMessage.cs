//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Priority queue message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using Newtonsoft.Json;

    /// <summary>
    ///     The Priority queue message.
    /// </summary>
    public class PriorityQueueMessage
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the email cargo
        /// </summary>
        [JsonProperty(PropertyName = "email_cargo")]
        public PriorityEmailCargo EmailCargo { get; set; }

        /// <summary>
        ///     Gets or sets the message id.
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        public string MessageId { get; set; }

        /// <summary>
        ///     Gets or sets the pop receipt.
        /// </summary>
        [JsonProperty(PropertyName = "pop_receipt")]
        public string PopReceipt { get; set; }

        #endregion

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Message Id: {0}; PopReceipt: {1}; Cargo : {2}", this.MessageId, this.PopReceipt, this.EmailCargo);
        }
    }
}