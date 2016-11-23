//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents MasterCard rebate confirmation exception reason codes.
    /// </summary>
    public enum ExceptionReasonCode
    {
        /// <summary>
        /// Indicates that the rebate failed but the reason code in the file was unrecognized.
        /// </summary>
        Unrecognized,

        /// <summary>
        /// Indicates the rebate failed because the account could not be found.
        /// </summary>
        AccountNotFound,

        /// <summary>
        /// Indicates the rebate failed because the customer could not be found.
        /// </summary>
        CustomerNotFound,

        /// <summary>
        /// Indicates the rebate failed because the account is invalid.
        /// </summary>
        InvalidAccount,

        /// <summary>
        /// Indicates the rebate failed because multiple matching accounts were found.
        /// </summary>
        MultipleAccountsFound,

        /// <summary>
        /// Indicates the rebate failed because of an unknown reason.
        /// </summary>
        Others,

        /// <summary>
        /// Indicates the rebate failed because the account is from an invalid country.
        /// </summary>
        InvalidAccountCountry 
    }
}