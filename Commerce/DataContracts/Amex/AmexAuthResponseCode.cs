//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    /// <summary>
    /// Represents Amex Auth Response Code.
    /// </summary>
    public class AmexAuthResponseCode
    {
        public const string AmexAuthSuccess = "0";
        public const string AmexAuthDealNotFound = "2";
        public const string AmexAuthInternalError = "4";
    }
}