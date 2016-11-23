//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The UnsubscribeService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.EmailSubscription.Service
{
    using System;
    using System.ServiceModel;

    using Microsoft.IT.RelationshipManagement.MDM.Platform.NewsletterUnsubscribe;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUnsubscribeService" in both code and config file together.
    
    /// <summary>
    /// The UnsubscribeService interface.
    /// </summary>
    [ServiceContract(Name = "IUnsubscribeService", Namespace = "http://schemas.microsoft.com/lomo/2013/01/IUnsubscribeService")]
    public interface IUnsubscribeService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The notify.
        /// </summary>
        /// <param name="obaUnsubscribe">
        /// The oba Unsubscribe.
        /// </param>
        [OperationContract(Action = "http://schemas.microsoft.com/lomo/2013/01/IUnsubscribeService/Notify", Name = "Notify")]
        void Notify(ObaUnsubscribe obaUnsubscribe);

        #endregion
    }

    /// <summary>
    /// The oba unsubscribe.
    /// </summary>
    [MessageContract(IsWrapped = true, WrapperName = "ObaUnsubscribe", WrapperNamespace = "http://Microsoft.IT.RelationshipManagement.EventNotification.Unsubscribe.Schemas.ObaUnsubscribe/v1")]
    public class ObaUnsubscribe 
    {
        /// <summary>
        /// Gets or sets the event instance id.
        /// </summary>
        [MessageBodyMember(Name = "EventInstanceId", Namespace = "", Order = 0)]
        public Guid EventInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the correlation id.
        /// </summary>
        [MessageBodyMember(Name = "CorrelationId", Namespace = "", Order = 1)]
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the correlation ticket.
        /// </summary>
        [MessageBodyMember(Name = "CorrelationTicket", Namespace = "", Order = 2)]
        public string CorrelationTicket { get; set; }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        [MessageBodyMember(Name = "EntityId", Namespace = "", Order = 3)]
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [MessageBodyMember(Name = "Body", Namespace = "", Order = 4)]
        public UnsubscribeRequestData Body { get; set; }
    }
}