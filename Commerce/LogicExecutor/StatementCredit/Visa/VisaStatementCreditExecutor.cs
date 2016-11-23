//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Linq;

namespace Lomo.Commerce.Logic
{
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.VisaClient;
    using System;

    /// <summary>
    ///     Contains logic necessary to process the Visa OnClear EndpointMessageRequest 
    /// </summary>
    public class VisaStatementCreditExecutor
    {
        private const string ErrorStatementCreditAlreadyIssuedForTransaction = "RTMUSCE0007";
        /// <summary>
        ///     Initializes a new instance of the VisaStatementCreditExecutor class.
        /// </summary>
        /// <param name="context">
        ///     The context for the API being invoked.
        /// </param>
        public VisaStatementCreditExecutor(CommerceContext context)
        {
            Context = context;
            //Context[Key.Partner] = Partner.Visa;
        }

        /// <summary>
        ///     Executes processing of the request.
        /// </summary>
        public ResultCode Execute()
        {
            const string statusFail = "0";
            
            var request = (EndPointMessageRequest)Context[Key.Request];
            var messageElementCollectionDictionary = request.MessageElementsCollection.ToDictionary(c => c.Key, c => c.Value, StringComparer.OrdinalIgnoreCase);
            var transactionId = messageElementCollectionDictionary[VisaEPMConstants.TransactionTransactionID];
            var status = messageElementCollectionDictionary[VisaEPMConstants.FulfillmentStatus];

            var creditStatus = CreditStatus.CreditGranted;
            if (status == statusFail)
            {
                creditStatus = CreditStatus.RejectedByPartner;
                if (messageElementCollectionDictionary.ContainsKey(VisaEPMConstants.FulfillmentStatusMessage))
                {
                    var errorMessage = messageElementCollectionDictionary[VisaEPMConstants.FulfillmentStatusMessage];
                    if (errorMessage.IndexOf(ErrorStatementCreditAlreadyIssuedForTransaction, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        creditStatus = CreditStatus.CreditGranted;
                        Context.Log.Verbose("Received statementCredit EPM message from Visa for partnerRedeemedDealScopeId={0} informing that credit for this transaction is already issued. Setting credit status to {1}.", transactionId, creditStatus);
                    }
                    else
                    {
                        Context.Log.Verbose("StatementCredit for Visa for partnerRedeemedDealScopeId={0} failed with error {1}", transactionId, errorMessage);
                    }
                }
            }

            DateTime processedDateTime = DateTime.UtcNow;
            if (messageElementCollectionDictionary.ContainsKey(VisaEPMConstants.FulfillmentProcessedDateTime))
            {
                var processedDateTimeString = messageElementCollectionDictionary[VisaEPMConstants.FulfillmentProcessedDateTime];
                // UTC Time: 2014-08-19T23:17:03.000Z
                processedDateTime = DateTime.Parse(processedDateTimeString);
                processedDateTime = DateTime.SpecifyKind(processedDateTime, DateTimeKind.Utc);
            }

            Context[Key.CreditStatus] = creditStatus;
            Context[Key.Transaction] = transactionId;
            Context[Key.TransactionCreditApprovedDate] = processedDateTime;

            Context.Log.Verbose("Updating redeemed deal having partnerRedeemedDealScopeId={0} to credit status {1}.", transactionId, creditStatus);
            var redeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            redeemedDealOperations.UpdateRedeemedDealsByVisa();
            Context.Log.Verbose("Updated redeemed deal having partnerRedeemedDealScopeId={0} to credit status {1}.", transactionId, creditStatus);
            return ResultCode.Success;
        }
        
        /// <summary>
        ///     Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}