//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace LoMo.UserServices.DealsMailing
{
    public class CampaignTemplateData : EmailTemplateData
    {
        /// <summary>
        /// Gets or sets the location name.
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Gets or sets content of the campaign email
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets if CLO business names need to be included in the mail
        /// </summary>
        public bool IncludeBusinessNames { get; set; }

        /// <summary>
        /// Gets or sets the campaign.
        /// </summary>
        public string Campaign { get; set; }
    }
}