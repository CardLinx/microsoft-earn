//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.VisaClient
{
    public class VisaConstants
    {
        /// <summary>
        /// Community name
        /// </summary>
        public const string CommunityCode = "MSN";

        /// <summary>
        /// Community name at group level
        /// </summary>
        public const string CommunityCodeGroupLevel = "MSNCG";

        /// <summary>
        /// Community name at CL level
        /// </summary>
        public const string CommunityCodeClLevel = "MSNCL";
    }
    

    public class VisaCallErrorConstants
    {
        public const string MerchantAlreadyOnboarded = "RTMMOBE0014";
        public const string DuplicateOfferName = "RTMCSCE0017";
        public const string OfferAlreadyExisitForGivenMidAndSid = "RTMMOBE0025";
    }
}