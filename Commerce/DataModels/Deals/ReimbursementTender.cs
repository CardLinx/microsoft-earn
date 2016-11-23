//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the means through which deal reimbursement will be tendered.
    /// </summary>
    public enum ReimbursementTender
    {
        /// <summary>
        /// Indicates the reimbursement will be tendered as an Earn credit.
        /// </summary>
        [XmlEnum("0")]
        MicrosoftEarn = 0,

        /// <summary>
        /// Indicates the reimbursement will be tendered as a Burn debit / statement credit.
        /// </summary>
        [XmlEnum("1")]
        MicrosoftBurn = 1,

        /// <summary>
        /// This used to be the Earn value, but it's now 0.
        /// </summary>
        [XmlEnum("2")]
        DeprecatedEarn = 2,

        /// <summary>
        /// This used to be the Burn value, but it's now 0.
        /// </summary>
        [XmlEnum("3")]
        DeprecatedBurn = 3,
    }
}