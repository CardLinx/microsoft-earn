//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Commerce.DataContracts;
using System.Net;

namespace OfferManagement.JobProcessor
{
    public interface ICommerceService
    {
        CommerceResponse RegisterOffer(string offerPayload);
    }
}