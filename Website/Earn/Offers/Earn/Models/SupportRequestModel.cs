//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;

namespace Earn.Offers.Earn.Models
{
    [DataContract]
    public class SupportRequestModel
    {
        [DataMember(Name = "full_name")]
        public string FullName
        {
            get;
            set;
        }

        [DataMember(Name="email")]
        public string Email
        {
            get;
            set;
        }

        [DataMember(Name = "assistance_with")]
        public string AssistanceWith
        {
            get;
            set;
        }

        [DataMember(Name = "last_4_digits")]
        public string Last4Digits
        {
            get;
            set;
        }

        [DataMember(Name = "card_type")]
        public string CardType
        {
            get;
            set;
        }

        [DataMember(Name = "card_brand")]
        public string CardBrand
        {
            get;
            set;
        }

        [DataMember(Name = "pin_entered")]
        public string PINEntered
        {
            get;
            set;
        }

        [DataMember(Name = "merchant_name")]
        public string MerchantName
        {
            get;
            set;
        }

        [DataMember(Name = "merchant_address")]
        public string MerchantAddress
        {
            get;
            set;
        }

        [DataMember(Name = "purchase_date")]
        public string PurchaseDate
        {
            get;
            set;
        }

        [DataMember(Name = "purchase_amount")]
        public string PurchaseAmount
        {
            get;
            set;
        }

        [DataMember(Name = "promotion")]
        public string Promotion
        {
            get;
            set;
        }

        [DataMember(Name = "details")]
        public string Details
        {
            get;
            set;
        }

        public override string ToString()
        {
            string result = string.Empty;

            result += "<div style='font-family: Segoe UI Web Regular; font-size: 18px; line-height: 19px;'>";
            result += "<p><b>Full Name: </b>" + this.FullName + "</p>";
            result += "<p><b>Email: </b>" + this.Email + "</p>";
            result += "<p><b>Assistance with: </b>" + this.AssistanceWith + "</p>";
            result += "<p><b>Last 4 Digits: </b> " + this.Last4Digits + "</p>";
            result += "<p><b>Card Brand: </b> " + this.CardBrand + "</p>";
            result += "<p><b>Card Type: </b>" + this.CardType + "</p>";

            if (this.CardType == "Debit Card")
            {
                result += "<p><b>PIN Entered: </b>" + this.PINEntered + "</p>";
            }
            
            result += "<p><b>Merchant Name: </b>" + this.MerchantName + "</p>";
            result += "<p><b>Purchase Address: </b>" + this.MerchantAddress + "</p>";
            result += "<p><b>Purchase Date: </b>" + this.PurchaseDate + "</p>";
            result += "<p><b>Purchase Amount: </b>" + this.PurchaseAmount + "</p>";
            result += "<p><b>Promotion: </b>" + this.Promotion + "</p>";
            result += "<p><b>Details: </b>" + this.Details + "</p>";
            result += "</div>";

            return result;
        }
    }
}