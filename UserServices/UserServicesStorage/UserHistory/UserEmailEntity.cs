//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The user email history entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.UserServices.Storage.UserHistory
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Type of email sent to the user
    /// </summary>
    public enum EmailType
    {
        /// <summary>
        /// Weekly Deals email
        /// </summary>
        WeeklyDeal,

        /// <summary>
        /// Remainder email to complete the sign up
        /// </summary>
        CompleteSignup
    }

    /// <summary>
    /// The user email history entity.
    /// </summary>
    public class UserEmailEntity : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserEmailEntity"/> class.
        /// </summary>
        public UserEmailEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserEmailEntity"/> class.
        /// </summary>
        /// <param name="userId">
        /// The user id. 
        /// </param>
        /// <param name="locationId">
        /// The location Id.
        /// </param>
        /// <param name="emailDate">
        /// The email date. 
        /// </param>
        /// <param name="emailType">The type of email</param>
        public UserEmailEntity(Guid userId, string locationId, DateTime emailDate, EmailType emailType)
        {
            PartitionKey = userId.ToString();
            var inverseTimeKey = DateTime
                                  .MaxValue
                                  .Subtract(emailDate)
                                  .TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            RowKey = string.Format("{0}-{1}", inverseTimeKey, Guid.NewGuid());
            EmailDate = emailDate;
            LocationId = locationId;
            EmailType = emailType.ToString();
        }

        /// <summary>
        /// Gets or sets the email date.
        /// </summary>
        public DateTime EmailDate { get; set; }

        /// <summary>
        /// Gets or sets the location id
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Gets or sets the type of email being sent to the user
        /// </summary>
        public string EmailType { get; set; }

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// The set serialized payload.
        /// </summary>
        /// <param name="payload">
        /// The payload.
        /// </param>
        public void SetSerializedPayload(UserEmailPayload payload)
        {
            var xmlSerializer = new XmlSerializer(typeof(UserEmailPayload));
            var sb = new StringBuilder();
            using (var messageWriter = new StringWriter(sb))
            {
                xmlSerializer.Serialize(messageWriter, payload);
                Payload = sb.ToString();
            }
        }

        /// <summary>
        /// The get deserialized payload.
        /// </summary>
        /// <returns>
        /// The <see cref="UserEmailPayload"/>.
        /// </returns>
        public UserEmailPayload GetDeserializedPayload()
        {
            var messageXmlSerializer = new XmlSerializer(typeof(UserEmailPayload));
            using (var messageReader = new StringReader(Payload))
            {
                var payload = (UserEmailPayload)messageXmlSerializer.Deserialize(messageReader);
                return payload;
            }
        }
    }
}