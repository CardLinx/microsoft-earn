//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System.Net.Http;

    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains helper methods for Controller classes.
    /// </summary>
    internal class ControllerHelper
    {

        /// <summary>
        /// The request contains a header that helps identify under which reward program's
        /// context the request is coming in.
        /// </summary>
        public static RewardPrograms GetRewardProgramAssociatedWithRequest(HttpRequestMessage requestMessage)
        {
            if (requestMessage.ContainsHeader(Constants.FlightIdHeaderName, Constants.FlightIdHeaderValueForEarn))
            {
                return RewardPrograms.EarnBurn;
            }

            return RewardPrograms.CardLinkOffers;
        }
    }
}