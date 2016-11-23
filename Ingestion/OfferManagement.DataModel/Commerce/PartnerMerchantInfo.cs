//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.DataModel.Commerce
{
    public class PartnerMerchantInfo
    {
        internal string PartnerMerchantId;
        internal string MerchantTimeZoneId;

        public PartnerMerchantInfo(string partnerMerchantId, string merchantTimeZoneId)
        {
            this.PartnerMerchantId = partnerMerchantId;
            this.MerchantTimeZoneId = merchantTimeZoneId;
        }
    }

}