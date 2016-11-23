//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;

namespace OfferManagement.DataModel.Partners.Visa
{
    public class VisaOffer
    {
        //it should be guid
        public string Id { get; set; }

        //can be set to merchant name
        public string Name { get; set; }

        //name of campaign as set in Visa portal which is generally the network provider name like "Rewards Network"
        public string CampaignName { get; set; }

        //offer start date
        public DateTime StartDate { get; set; }

        //offer end date
        public DateTime EndDate { get; set; }

        //this may be a constant like "MSNCGMG"
        public string Template { get; set; }

        //this may be a constant like "840"  or "UNITED STATES" for US
        public string CountryCode { get; set;}

        //Mids correspond to single merchant only
        //public IList<VisaMerchant> Merchants { get; set;}

        //need to check if we need to set this
        //public string RecurrenceLimit { get; set;}
    }
}