//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.VisaClient
{
    public enum MerchantSearchType
    {
        SearchUsingParametersSupplied = 0,
        SearchUsingWildCardInName = 1,
        SearchUsingBingAddress = 2,
        SearchWithoutStreetAddress = 3
    }
}
