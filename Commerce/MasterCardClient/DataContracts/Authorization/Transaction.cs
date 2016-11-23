//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;
    using System.CodeDom.Compiler;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a MasterCard authorization transaction.
    /// </summary>
    [GeneratedCodeAttribute("webAPI", "4.0")]
    [DataContract]
    public class Transaction
    {
        /// <summary>
        /// Gets or sets the time at which MasterCard recorded the transaction.
        /// </summary>
        [DataMember]
        public DateTime timestamp { get; set; }

        /// <summary>
        /// Gets or sets the amount of the transaction.
        /// </summary>
        [DataMember]
        public decimal transAmt { get; set; }

        /// <summary>
        /// Gets or sets the Microsoft-assigned ID for the card account used in the transaction.
        /// </summary>
        [DataMember]
        public string bankCustNum { get; set; }

        /// <summary>
        /// Gets or sets the Bank NET reference number for the account used in the transaction.
        /// </summary>
        [DataMember]
        public string refNum { get; set; }

        /// <summary>
        /// Gets or sets the date on which the transaction occurred.
        /// </summary>
        public string transDate { get; set; }

        /// <summary>
        /// Gets or sets the time at which the transaction occurred.
        /// </summary>
        public string transTime { get; set; }

        /// <summary>
        /// Gets or sets the ICA of the acquirer from which the merchant ID was sourced.
        /// </summary>
        [DataMember]
        public string acquirerIca { get; set; }

        /// <summary>
        /// Gets or sets the ID of the merchant at which the transaction occurred.
        /// </summary>
        [DataMember]
        public string merchId { get; set; }
    }
}