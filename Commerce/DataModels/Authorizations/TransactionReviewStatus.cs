//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the the possible review status states in which a transaction may be placed.
    /// </summary>
    public enum TransactionReviewStatus
    {
        /// <summary>
        /// Indicates no transaction review is currently deemed necessary.
        /// </summary>
        [XmlEnum("0")]
        Unnecessary = 0,

        /// <summary>
        /// Indicates the transaction should be reviewed becaue the transaction amount may be suspicious, e.g. may be an indication of fraudulent activity.
        /// </summary>
        [XmlEnum("1")]
        SuspiciousTransactionAmount = 1,

        /// <summary>
        /// Indicates the transaction review was resolved by accepting the transaction. The transaction will continue to flow through the pipeline as usual.
        /// </summary>
        [XmlEnum("2")]
        ResolvedAccept = 2,

        /// <summary>
        /// Indicates the transaction review was resolved by rejecting the transaction. The transaction credit status will be placed into a terminal state.
        /// </summary>
        [XmlEnum("3")]
        ResolvedReject = 3
    }
}