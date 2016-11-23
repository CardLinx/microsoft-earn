//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents a user stored in the data store.
    /// </summary>f
    public class User
    {
        /// <summary>
        /// Initializes a new instance of the User class.
        /// </summary>
        public User()
        {
            PartnerUserInfoList = new Collection<PartnerUserInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the User class, using the specified ID.
        /// </summary>
        /// <param name="userId">
        /// ID to assign to the User object.
        /// </param>
        public User(Guid userId)
            : this()
        {
            GlobalId = userId;
        }

        /// <summary>
        /// Initializes a new instance of the User class, using the specified ID.
        /// </summary>
        /// <param name="userId">
        /// ID to assign to the User object.
        /// </param>
        /// <param name="analyticsEventId">
        /// The event ID for the analytics system assigned upon creation of this user.
        /// </param>
        public User(Guid userId, Guid analyticsEventId)
            : this(userId)
        {
            AnalyticsEventId = analyticsEventId;
        }

        /// <summary>
        /// Initializes a new instance of the User class, using the fields from the specified other User.
        /// </summary>
        /// <param name="user">
        /// The other User whose fields to copy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter user cannot be null.
        /// </exception>
        public User(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "Parameter user cannot be null.");
            }

            Id = user.Id;
            GlobalId = user.GlobalId;
            AnalyticsEventId = user.AnalyticsEventId;
            PartnerUserInfoList = new Collection<PartnerUserInfo>();
            foreach(PartnerUserInfo partnerUserInfo in user.PartnerUserInfoList)
            {
                PartnerUserInfoList.Add(new PartnerUserInfo { PartnerId = partnerUserInfo.PartnerId,
                                                              PartnerUserId = partnerUserInfo.PartnerUserId });
            }
        }

        /// <summary>
        /// Gets partner user info list. Appends explicitly stored with derived values
        /// </summary>
        /// <param name="partner">
        /// the partner for filtering
        /// </param>
        /// <returns>
        /// partner user id
        /// </returns>
        public string GetPartnerUserId(Partner partner)
        {
            PartnerUserInfo info = PartnerUserInfoList.FirstOrDefault(_ => _.PartnerId == partner);
            return info == null ? UserIdHexFormat : info.PartnerUserId;
        }

        /// <summary>
        /// Add paratner userId and if it already exist then updates it
        /// </summary>
        /// <param name="partner">the partner for filtering</param>
        /// <param name="partnerUserId">Partner user Id</param>
        /// <param name="assignedByPartner">Value that indicates whether the partner user ID was assigned by the partner.</param>
        public void AddOrUpdatePartnerUserId(Partner partner, string partnerUserId, bool assignedByPartner)
        {
            PartnerUserInfo info = PartnerUserInfoList.FirstOrDefault(_ => _.PartnerId == partner);
            if (info == null)
            {
                info = new PartnerUserInfo
                {
                    PartnerUserId = partnerUserId,
                    PartnerId = partner,
                    AssignedByPartner = assignedByPartner
                };

                PartnerUserInfoList.Add(info);
            }
            else
            {
                info.PartnerUserId = partnerUserId;
                info.AssignedByPartner = assignedByPartner;
            }
        }

        /// <summary>
        /// Determines whether the specified object has equal values to this object in all fields.
        /// </summary>
        /// <param name="obj">
        /// The object whose values to compare.
        /// </param>
        /// <returns>
        /// True if the two objects have the same values.
        /// </returns>
        /// <remarks>
        /// Rounding timestamps to the nearest second is sufficient and accounts for lower precision in SQL datetime.
        /// </remarks>
        public override bool Equals(object obj)
        {
            User user = (User)obj;
            return Id == user.Id &&
                   GlobalId == user.GlobalId &&
                   AnalyticsEventId == user.AnalyticsEventId &&
                   PartnerUserInfoList.Except(user.PartnerUserInfoList).Any() == false &&
                   user.PartnerUserInfoList.Except(PartnerUserInfoList).Any() == false;
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns>
        /// The hash code for this object.
        /// </returns>
        /// <remarks>
        /// * CA2218:
        ///   * If two objects are equal in value based on the Equals override, they must both return the same value for calls
        ///     to GetHashCode.
        ///   * GetHashCode must be overridden whenever Equals is overridden.
        /// * It is fine if the value overflows.
        /// </remarks>
        public override int GetHashCode()
        {
            int result = Id.GetHashCode() +
                         GlobalId.GetHashCode() +
                         AnalyticsEventId.GetHashCode();

            if (PartnerUserInfoList != null)
            {
                foreach(PartnerUserInfo partnerUserInfo in PartnerUserInfoList)
                {
                    result += partnerUserInfo.GetHashCode();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the canonical ID for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the canonical ID for the user.
        /// </summary>
        public Guid GlobalId { get; set; }

        /// <summary>
        /// Gets or sets the event ID for the analytics system assigned upon creation of this user.
        /// </summary>
        public Guid AnalyticsEventId { get; set; }

        /// <summary>
        /// Gets or sets a list of information about the user from partners.
        /// </summary>
        public Collection<PartnerUserInfo> PartnerUserInfoList { get; private set; }

        /// <summary>
        /// Gets or sets partner user Id based on integer user Id
        /// </summary>
        public string UserIdHexFormat
        {
            get
            {
                if (Id == 0)
                {
                    throw new InvalidOperationException("Unexpected value in Id");
                }

                return Id.ToString("X");
            }
        }
    }
}