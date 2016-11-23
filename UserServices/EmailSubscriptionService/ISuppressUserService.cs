//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The suppress operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.EmailSubscription.Service
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    /// <summary>
    /// The SuppressUserService interface.
    /// </summary>
    [ServiceContract(Name = "ISuppressUserService", Namespace = "http://schemas.microsoft.com/lomo/2013/01/ISuppressUserService")]
    public interface ISuppressUserService
    {
        /// <summary>
        /// The notify.
        /// </summary>
        /// <param name="obaEmailPermissionUpdate">
        /// The oba Email Permission Update.
        /// </param>
        [OperationContract(Action = "http://schemas.microsoft.com/lomo/2013/01/ISuppressUserService/Notify", Name = "Notify")]
        void Notify(ObaEmailPermissionUpdateV2 obaEmailPermissionUpdate);
    }

    // Type: Microsoft.IT.RelationshipManagement.MDM.Adapter.DataEntities.ObaEmailPermissionUpdateV2

    // Assembly: MDM.Adapter, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
    // Assembly location: C:\Dev\HCP_New\FSAA\MDM.Adapter.dll

    /// <summary>
    /// The oba email permission update v 2.
    /// </summary>
    [MessageContract(WrapperName = "ObaEmailPermissionUpdateV2", WrapperNamespace = "http://Microsoft.IT.RelationshipManagement.EventNotification.Mio.Privacy.EmailPermissionUpdate/V2")]
    public class ObaEmailPermissionUpdateV2
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
        /// Gets or sets the entity id.
        /// </summary>
        [MessageBodyMember(Name = "EntityId", Namespace = "", Order = 2)]
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [MessageBodyMember(Name = "Body", Namespace = "", Order = 3)]
        public EmailPrivacyResponseData Body { get; set; }
    }

    /// <summary>
    /// The email privacy response data.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    [DataContract(Name = "EmailPrivacyResponseData", Namespace = "http://schemas.microsoft.com/microsoft-services/mdm/2009/02")]
    public class EmailPrivacyResponseData
    {
        /// <summary>
        /// Gets or sets the email privacy data.
        /// </summary>
        [DataMember]
        public EmailPrivacyData EmailPrivacyData { get; set; }
    }

    /// <summary>
    /// The email privacy data.
    /// </summary>
    [DataContract(Name = "EmailPrivacyData", Namespace = "http://schemas.microsoft.com/microsoft-services/mdm/2009/02")]
    public class EmailPrivacyData
    {
        /// <summary>
        /// Gets or sets the country id.
        /// </summary>
        [DataMember]
        public int CountryID { get; set; }

        /// <summary>
        /// Gets or sets the country id native last modified date time.
        /// </summary>
        [DataMember]
        public DateTime CountryIDNativeLastModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the deliverability id.
        /// </summary>
        [DataMember]
        public short? DeliverabilityID { get; set; }

        /// <summary>
        /// Gets or sets the deliverability native last modified date time.
        /// </summary>
        [DataMember]
        public DateTime? DeliverabilityNativeLastModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the email address text.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string EmailAddressText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active flag.
        /// </summary>
        [DataMember]
        public bool IsActiveFlag { get; set; }

        /// <summary>
        /// Gets or sets the ms research use ms wide privacy permission id.
        /// </summary>
        [DataMember]
        public short MSResearchUseMSWidePrivacyPermissionID { get; set; }

        /// <summary>
        /// Gets or sets the ms research use native last modified date time.
        /// </summary>
        [DataMember]
        public DateTime MSResearchUseNativeLastModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the object status reason id.
        /// </summary>
        [DataMember]
        public byte ObjectStatusReasonID { get; set; }

        /// <summary>
        /// Gets or sets the promotional use ms wide privacy permission id.
        /// </summary>
        [DataMember]
        public short PromotionalUseMSWidePrivacyPermissionID { get; set; }

        /// <summary>
        /// Gets or sets the promotional use native last modified date time.
        /// </summary>
        [DataMember]
        public DateTime PromotionalUseNativeLastModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the suppressed for all contact effective date.
        /// </summary>
        [DataMember]
        public DateTime? SuppressedForAllContactEffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the suppressed for all contact end date.
        /// </summary>
        [DataMember]
        public DateTime? SuppressedForAllContactEndDate { get; set; }

        /// <summary>
        /// Gets or sets the suppressed for all contact id.
        /// </summary>
        [DataMember]
        public short? SuppressedForAllContactID { get; set; }

        /// <summary>
        /// Gets or sets the suppressed for all contact reason id.
        /// </summary>
        [DataMember]
        public short? SuppressedForAllContactReasonID { get; set; }

        /// <summary>
        /// Gets or sets the third party transfer ms wide privacy permission id.
        /// </summary>
        [DataMember]
        public short ThirdPartyTransferMSWidePrivacyPermissionID { get; set; }

        /// <summary>
        /// Gets or sets the third party transfer native last modified date time.
        /// </summary>
        [DataMember]
        public DateTime ThirdPartyTransferNativeLastModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the undeliverable reason id.
        /// </summary>
        [DataMember]
        public short? UndeliverableReasonID { get; set; }
    }
}