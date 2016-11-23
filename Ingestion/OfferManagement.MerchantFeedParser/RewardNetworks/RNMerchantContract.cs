//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.MerchantFeedParser.RewardNetworks
{
    using System.Collections.Generic;

    public class RewardNetworkMerchant
    {
        public string status { get; set; }
        public List<MerchantAttribute> merchantAttributes { get; set; }
        public List<object> merchantBenefits { get; set; }
        public MerchantContent merchantContent { get; set; }
        public MerchantDetails merchantDetails { get; set; }
        public List<MerchantReview> merchantReviews { get; set; }
    }

    public class MerchantAttribute
    {
        public string attrDesc { get; set; }
        public string categoryDesc { get; set; }
    }

    public class Hour
    {
        public string dayOfWeek { get; set; }
        public string open { get; set; }
        public string close { get; set; }
    }

    public class MerchantContent
    {
        public string description { get; set; }
        public object facebookUrl { get; set; }
        public List<Hour> hours { get; set; }
        public string logo { get; set; }
        public int memberFavCount { get; set; }
        public List<string> menus { get; set; }
        public List<string> photos { get; set; }
        public List<string> realPhotos { get; set; }
        public string twitterUrl { get; set; }
        public string url { get; set; }
    }

    public class MerchantDetails
    {
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public List<string> cuisines { get; set; }
        public string entreePrices { get; set; }
        public int merchantId { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string memberRating { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string ratingAverage { get; set; }
        public string ratingCleanliness { get; set; }
        public string ratingExperience { get; set; }
        public string ratingFood { get; set; }
        public string ratingService { get; set; }
        public string ratingValue { get; set; }
        public string state { get; set; }
        public int totalReviews { get; set; }
        public string zipcode { get; set; }
    }

    public class MerchantReview
    {
        public int answerId { get; set; }
        public string escapedReview { get; set; }
        public string formattedReviewDate { get; set; }
        public int helpfulMerchantResponseCount { get; set; }
        public int helpfulReviewCount { get; set; }
        public string memberCity { get; set; }
        public string memberInitials { get; set; }
        public string memberName { get; set; }
        public string memberState { get; set; }
        public string merchantResponseDate { get; set; }
        public bool merchantResponseFlag { get; set; }
        public int merchantResponseStatus { get; set; }
        public bool moreReviewsForMember { get; set; }
        public string overallRating { get; set; }
        public int responseId { get; set; }
        public int totalMerchantResponseCount { get; set; }
        public int totalResponseCount { get; set; }
    }
}