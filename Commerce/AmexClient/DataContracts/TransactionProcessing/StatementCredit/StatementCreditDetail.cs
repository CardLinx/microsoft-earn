//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System.Text;

    public class StatementCreditDetail
    {
        /// <summary>
        /// Gets or sets Transaction Id
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the card token
        /// </summary>
        public string CardToken { get; set; }

        /// <summary>
        /// Gets or sets the Discount Amount
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the campaign name
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// Gets or sets the offer id
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Statement Descriptor - Upto 16 characters
        /// </summary>
        public  string StatementDescriptor { get; set; }

        /// <summary>
        /// Construct a line record for statement credit file
        /// </summary>
        /// <returns>
        /// String representation of the detail record
        /// </returns>
        public string BuildFileDetailRecord()
        {
            StringBuilder result = new StringBuilder();
            result.Append(AmexConstants.DetailIdentifier);
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.PartnerId);
            result.Append(AmexConstants.Delimiter);
            result.Append(FormattedTransactionId());
            result.Append(AmexConstants.Delimiter);
            result.Append(CardToken);
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.DiscountIndicator);
            result.Append(AmexConstants.Delimiter);
            result.Append(FormattedDiscountAmount());
            result.Append(AmexConstants.Delimiter);
            result.Append(RewardPoints);
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.Currency);
            result.Append(AmexConstants.Delimiter);
            result.Append(CampaignName);
            result.Append(AmexConstants.Delimiter);
            result.Append(OfferId);
            result.Append(AmexConstants.Delimiter);
            result.Append(StatementDescriptor);
            result.Append(AmexConstants.Delimiter);
            result.Append(Filler);
            return result.ToString();
        }

        /// <summary>
        /// Formatted amount Decimal(15,2) with sign
        /// </summary>
        /// <returns>
        /// Formatted amount
        /// </returns>
        private string FormattedDiscountAmount()
        {
            const string fmt = "0000000000000.##";
            string formattedAmount = DiscountAmount.ToString(fmt);
            if (DiscountAmount > 0)
            {
                formattedAmount = "+" + formattedAmount;
            }
            return formattedAmount;
        }

        private string FormattedTransactionId()
        {
            const string fmt = "000000000";
            int transactionId = int.Parse(TransactionId);
            string formattedtransactionId = transactionId.ToString(fmt);
            return formattedtransactionId;
        }
        
        /// <summary>
        /// Reward Points 
        /// </summary>
        private string RewardPoints = "000000000";

        /// <summary>
        /// Filler
        /// </summary>
        private string Filler = " ";
    }
}