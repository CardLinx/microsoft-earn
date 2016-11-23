//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    using Newtonsoft.Json;
    using Users.Dal;
    using Users.Dal.DataModel;

    /// <summary>
    /// Add Card Email Payload to be sent to Template Service to get Templated Content
    /// </summary>
    public class AddCardEmailNotificationData
    {
        /// <summary>
        /// Gets or sets Last Four Digits
        /// </summary>
        [JsonProperty(PropertyName = "last_four_digits")]
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets Card Type
        /// </summary>
        [JsonProperty(PropertyName = "card_type")]
        public string CardType { get; set; }

        /// <summary>
        /// Gets or sets the unathenticated user status.
        /// </summary>
        [JsonProperty(PropertyName = "unauthenticated_user_status")]
        public string UnauthenticatedUserStatus { get; set; }

        /// <summary>
        /// Populate Authentication Status for a given user.
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
        public void PopulateAuthStatus(User user)
        {
            if (user != null)
            {
                if (user.IsEmailConfirmed)
                {
                    UnauthenticatedUserStatus = string.IsNullOrWhiteSpace(user.MsId) ? UnlinkedEmail : null;
                }
                else
                {
                    UnauthenticatedUserStatus = UnconfirmedEmail;
                }
            }
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