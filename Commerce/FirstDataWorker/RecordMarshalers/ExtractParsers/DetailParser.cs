//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Base class for detail parser classes.
    /// </summary>
    public abstract class DetailParser
    {
        /// <summary>
        /// Gets or sets the name of the file from which the record originated.
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// Determines the transaction type from the specified string.
        /// </summary>
        /// <param name="receivedValue">
        /// The field from which the transaction type will be determined.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="transactionType">
        /// Receives the transaction type if possible.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the transaction type could be determined from the specified string.
        /// * Else returns false.
        /// </returns>
        internal bool DetermineTransactionType(string receivedValue,
                                               string fieldName,
                                               out TransactionType transactionType,
                                               bool recordValid)
        {
            transactionType = TransactionType.RealTimeRedemption;
            if (recordValid == true)
            {
                switch (receivedValue.ToUpperInvariant())
                {
                    case "REAL-TIME REDEMPTION":
                        transactionType = TransactionType.RealTimeRedemption;
                        break;
                    case "SETTLEMENT REDEMPTION":
                        transactionType = TransactionType.SettlementRedemption;
                        break;
                    case "REAL-TIME REDEMPTION REVERSAL - TIMEOUT":
                        transactionType = TransactionType.RealTimeTimeoutReversal;
                        break;
                    case "REAL-TIME REDEMPTION REVERSAL - NON-TIMEOUT":
                        transactionType = TransactionType.RealTimeNonTimeoutReversal;
                        break;
                    case "SETTLEMENT REDEMPTION REVERSAL - TIMEOUT":
                        transactionType = TransactionType.SettlementTimeoutReversal;
                        break;
                    case "SETTLEMENT REDEMPTION REVERSAL - NON-TIMEOUT":
                        transactionType = TransactionType.SettlementNonTimeoutReversal;
                        break;
                    default:
                        Log.Warning("Error parsing \"{0}\" record #{1} from file \"{2}\". Received value \"{3}\" in field " +
                                    "\"{4}\" is not properly formatted.", (int)ResultCode.InvalidValue, LogRecordType,
                                    RecordNumber, FileName, receivedValue, fieldName);
                        recordValid = false;
                        break;
                };
            }

            return recordValid;
        }

        /// <summary>
        /// Gets or sets the type of record being parsed as it will appear in logs.
        /// </summary>
        protected string LogRecordType { get; set; }

        /// <summary>
        /// Gets or sets the number of the record being parsed.
        /// </summary>
        protected int RecordNumber { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        protected CommerceLog Log { get; set; }
    }
}