//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The unsubscribe request context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.EmailSubscription.Service
{
    using System;

    /// <summary>
    /// The unsubscribe request context.
    /// </summary>
    internal class RequestContext
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the correlation id.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the correlation ticket.
        /// </summary>
        public string CorrelationTicket { get; set; }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the event instance id.
        /// </summary>
        public Guid EventInstanceId { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[Event Instance Id={0}; Correlation Id={1}; Correlation Ticket={2}; Entity Id={3}]", this.EventInstanceId, this.CorrelationId, this.CorrelationTicket, this.EntityId);
        }

        #endregion
    }
}