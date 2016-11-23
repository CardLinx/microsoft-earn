//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    using System;
    using Newtonsoft.Json;
    using Users.Dal;
    using Users.Dal.DataModel;

    /// <summary>
    /// Auth Email Payload to be sent to Template Service to get Templated Content
    /// </summary>
    public class AuthEmailNotificationData
    {
        /// <summary>
        /// Get or sets username
        /// </summary>
        [JsonProperty(PropertyName = "user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// Get or sets merchantname
        /// </summary>
        [JsonProperty(PropertyName = "merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// Get or sets discount summary
        /// </summary>
        [JsonProperty(PropertyName = "discount_summary")]
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Get or sets credit amount
        /// </summary>
        [JsonProperty(PropertyName = "credit_amount")]
        public string CreditAmount { get; set; }

        /// <summary>
        /// Get or sets last 4 digits
        /// </summary>
        [JsonProperty(PropertyName = "last_four_digits")]
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the unathenticated user status.
        /// </summary>
        [JsonProperty(PropertyName = "unauthenticated_user_status")]
        public string UnauthenticatedUserStatus { get; set; }

        /// <summary>
        /// Gets or sets the account linking uri.
        /// </summary>
        [JsonProperty(PropertyName = "account_linking_url")]
        public Uri AccountLinkingUri { get; set; }

        /// <summary>
        /// Gets or sets the email verification uri.
        /// </summary>
        [JsonProperty(PropertyName = "email_verification_url")]
        public Uri EmailVerificationUri { get; set; }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        [JsonProperty(PropertyName = "transaction_id")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets transaction date.
        /// </summary>
        [JsonProperty(PropertyName = "transaction_date")]
        public string TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Partner Merchant Id.
        /// </summary>
        [JsonProperty(PropertyName = "partner_merchant_id")]
        public string PartnerMerchantId { get; set; }

        /// <summary>
        /// Gets or sets Partner Id.
        /// </summary>
        [JsonProperty(PropertyName = "partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the deal Id (Parent).
        /// </summary>
        [JsonProperty(PropertyName = "deal_id")]
        public string DealId { get; set; }

        /// <summary>
        /// Gets or sets the discount id (global).
        /// </summary>
        [JsonProperty(PropertyName = "discount_id")]
        public string DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the percent of the discount offered within the deal.
        /// </summary>
        [JsonProperty(PropertyName = "percent")]
        public float Percent { get; set; }

        /// <summary>
        /// Populate Authentication Status and email verification/confirmation link for a given user.
        /// - If the email is unconfirmed, auth status: UnConfirmedEmail
        /// - If email is confirmed, but MsId not populated, auth status : UnLinkedEmail
        /// - Otherwise we leave it as default: Null
        /// </summary>
        /// <param name="user">
        /// User object from Dal
        /// </param>
        /// <param name="dal">
        /// Instance of Users Dal
        /// </param>
        /// <param name="environmentType">
        /// Environment Type
        /// </param>
        public void PopulateAuthStatusAndEmailLink(User user, IUsersDal dal, EnvironmentType environmentType)
        {
            if (user != null)
            {
                if (user.IsEmailConfirmed)
                {
                    if (string.IsNullOrWhiteSpace(user.MsId))
                    {
                        UnauthenticatedUserStatus = UnlinkedEmail;
                        AccountLinkingUri = LinkingOrVerificationUrl(EntityType.AccountLink, user, dal, environmentType);
                    }
                    else
                    {
                        UnauthenticatedUserStatus = null;
                    }
                }
                else
                {
                    UnauthenticatedUserStatus = UnconfirmedEmail;
                    EmailVerificationUri = LinkingOrVerificationUrl(EntityType.UnAuthenticatedEmailAddress, user, dal, environmentType);
                }
            }
        }

        /// <summary>
        /// Get the link to be embedded in auth email if necessary.
        /// </summary>
        /// <param name="entityType">
        /// Entity Type for link generation
        /// </param>
        /// <param name="user">
        /// User object
        /// </param>
        /// <param name="environmentType">
        /// Environment Type
        /// </param>
        /// <returns>
        /// Returns the Uri.
        /// </returns>
        private static Uri LinkingOrVerificationUrl(EntityType entityType, User user, IUsersDal dal, EnvironmentType environmentType)
        {
            Tuple<string, int> confirmationCodeTuple = dal.CreateConfirmationCode(user, entityType);
            return ConfirmationLinkGenerator.GetLink(confirmationCodeTuple.Item1, confirmationCodeTuple.Item2, entityType, environmentType);
        }


        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns>
        /// string representation of the data
        /// </returns>
        public override string ToString()
        {
            return string.Format("Username : {0} \r\n" +
                                 "Merchantname : {1} \r\n" +
                                 "Discount Summary: {2} \r\n" +
                                 "Credit Amount : {3} \r\n" +
                                 "Last 4 Digits: {4}", UserName, MerchantName, DiscountSummary, CreditAmount,
                                 LastFourDigits);
        }

        /// <summary>
        /// Value for unconfirmed email status
        /// </summary>
        private readonly string UnconfirmedEmail = "UnConfirmedEmail";

        /// <summary>
        /// Value for unlinked email status
        /// </summary>
        private readonly string UnlinkedEmail = "UnLinkedEmail";
    }
}