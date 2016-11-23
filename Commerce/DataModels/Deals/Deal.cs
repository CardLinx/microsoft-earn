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
    using System.Xml.Linq;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents a deal stored in the data store.
    /// </summary>
    public class Deal
    {
        /// <summary>
        /// Initializes a new instance of the Deal class for serialization.
        /// </summary>
        public Deal()
        {
            ProviderId = String.Empty;
            MerchantId = String.Empty;
            ProviderCategory = String.Empty;
            PartnerDealInfoList = new Collection<PartnerDealInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the Deal class, using the fields from the specified other Deal.
        /// </summary>
        /// <param name="deal">
        /// The other Deal whose fields to copy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter deal cannot be null.
        /// </exception>
        public Deal(Deal deal)
        {
            if (deal == null)
            {
                throw new ArgumentNullException("deal", "Parameter deal cannot be null.");
            }

            GlobalId = deal.GlobalId;
            Id = deal.Id;
            ParentDealId = deal.ParentDealId;
            ProviderId = deal.ProviderId;
            MerchantId = deal.MerchantId;
            ProviderCategory = deal.ProviderCategory;
            MerchantName = deal.MerchantName;
            StartDate = deal.StartDate;
            EndDate = deal.EndDate;
            Currency = deal.Currency;
            Amount = deal.Amount;
            Percent = deal.Percent;
            MinimumPurchase = deal.MinimumPurchase;
            Count = deal.Count;
            UserLimit = deal.UserLimit;
            DiscountSummary = deal.DiscountSummary;
            MaximumDiscount = deal.MaximumDiscount;
            PartnerDealInfoList = new Collection<PartnerDealInfo>();
            DealStatusId = deal.DealStatusId;
            DayTimeRestrictions = deal.DayTimeRestrictions;
            foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
            {
                PartnerDealInfoList.Add(new PartnerDealInfo(partnerDealInfo));
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
        public override bool Equals(object obj)
        {
            Deal deal = (Deal) obj;
            return GlobalId == deal.GlobalId &&
                   ParentDealId == deal.ParentDealId &&
                   ProviderId == deal.ProviderId &&
                   MerchantId == deal.MerchantId &&
                   ProviderCategory == deal.ProviderCategory &&
                   MerchantName == deal.MerchantName &&
                   General.DateTimesComparable(StartDate, deal.StartDate) == true &&
                   General.DateTimesComparable(EndDate, deal.EndDate) == true &&
                   Currency == deal.Currency &&
                   Amount == deal.Amount &&
                   MinimumPurchase == deal.MinimumPurchase &&
                   Count == deal.Count &&
                   UserLimit == deal.UserLimit &&
                   DiscountSummary == deal.DiscountSummary &&
                   MaximumDiscount == deal.MaximumDiscount &&
                   DealStatusId == deal.DealStatusId &&
                   PartnerDealInfoList.Except(deal.PartnerDealInfoList).Any() == false &&
                   deal.PartnerDealInfoList.Except(PartnerDealInfoList).Any() == false &&
                   DayTimeRestrictions == deal.DayTimeRestrictions;
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
            int result = GlobalId.GetHashCode() +
                         ParentDealId.GetHashCode() +
                         ProviderId.GetHashCode() +
                         MerchantId.GetHashCode() +
                         ProviderCategory.GetHashCode() +
                         StartDate.GetHashCode() +
                         EndDate.GetHashCode() +
                         Amount.GetHashCode() +
                         Count.GetHashCode() +
                         UserLimit.GetHashCode() +
                         MinimumPurchase.GetHashCode() +
                         MaximumDiscount.GetHashCode() +
                         DealStatusId.GetHashCode();

            if (MerchantName != null)
            {
                result += MerchantName.GetHashCode();
            }

            if (Currency != null)
            {
                result += Currency.GetHashCode();
            }

            if (DiscountSummary != null)
            {
                result += DiscountSummary.GetHashCode();
            }

            foreach (PartnerDealInfo partnerDealInfo in PartnerDealInfoList)
            {
                result += partnerDealInfo.GetHashCode();
            }

            if (DayTimeRestrictions != null)
            {
                result += DayTimeRestrictions.GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Sets day-time restrictions xml based on the data passed in the deal data contract
        /// </summary>
        /// <param name="restrictions"></param>
        public void SetDayTimeRestrictions(IEnumerable<DayTimeRestriction> restrictions)
        {
            DayTimeRestrictions = null;
            if (restrictions != null && restrictions.Any())
            {
                DayTimeRestrictions = 
                    new XElement("Restrictions",
                                  restrictions.Select(_ => new XElement("Restriction",
                                                                         new XElement("StartDayAndTime", _.StartDayAndTime),
                                                                         new XElement("EndDayAndTime", _.EndDayAndTime))));
            }
        }

        /// <summary>
        /// Gets or sets the canonical ID for this deal.
        /// </summary>
        public Guid GlobalId { get; set; }

        /// <summary>
        /// Gets or sets the integer ID for this deal.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of this deal's parent.
        /// </summary>
        public Guid ParentDealId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the merchant's provider.
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the merchant offering the deal.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the provider's category.
        /// </summary>
        public string ProviderCategory { get; set; }

        /// <summary>
        /// Gets or sets the name of the merchant offering the deal.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the date at which the deal offer begins.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the date at which the deal offer ends.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the currency used within the deal, e.g. USD.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the means through which deal reimbursement will be tendered, e.g. Currency or CSV.
        /// </summary>
        public ReimbursementTender ReimbursementTender { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount offered within the deal.
        /// </summary>
        /// <remarks>
        /// * This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// * Setting both Amount and Percent is permitted, but different partners may resolve the conflict differently.
        /// </remarks>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the percent of the discount offered within the deal.
        /// </summary>
        /// <remarks>
        /// Setting both Amount and Percent is permitted, but different partners may resolve the conflict differently.
        /// </remarks>
        public decimal Percent { get; set; }

        /// <summary>
        /// Gets or sets the minimum purchase amount to cause the redemption to occur.
        /// </summary>
        /// <remarks>
        /// This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// </remarks>
        public int MinimumPurchase { get; set; }

        /// <summary>
        /// Gets or sets the total number of times a deal may be redeemed before becoming inactive.
        /// </summary>
        /// <remarks>
        /// Setting this value to 0 indicates no limit to the number of times the deal may be redeemed.
        /// </remarks>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the limit to the number of times a user may redeem the deal.
        /// </summary>
        /// <remarks>
        /// Setting this value to 0 indicates no limit to the number of times a user may redeem a deal.
        /// </remarks>
        public int UserLimit { get; set; }

        /// <summary>
        /// Gets or sets the summary description of the deal.
        /// </summary>
        /// <remarks>
        /// This string may appear on the user's receipt, credit card statement, or both.
        /// </remarks>
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets the maximum discount that will be granted for this deal.
        /// </summary>
        public int MaximumDiscount { get; set; }

        /// <summary>
        /// Gets or sets a list of information about the deal from partners.
        /// </summary>
        public Collection<PartnerDealInfo> PartnerDealInfoList { get; private set; }

        /// <summary>
        /// Gets or sets the deal status ID for the deal.
        /// </summary>
        public DealStatus DealStatusId { get; set; }

        /// <summary>
        /// Gets or sets the DayTimeRestrictions xml.
        /// </summary>
        public XElement DayTimeRestrictions { get; set; }
    }
}