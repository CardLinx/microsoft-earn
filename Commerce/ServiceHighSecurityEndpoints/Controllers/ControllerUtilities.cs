//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System;
    using System.Net.Http;

    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains utility methods for HBI service controllers.
    /// </summary>
    internal static class ControllerUtilities
    {
        /// <summary>
        /// Generates a legacy NewCardInfo object for the specified credit card number.
        /// </summary>
        /// <param name="newCardNumber">
        /// The object containing the number for which to generate a legacy NewCardInfo object.
        /// </param>
        /// <returns>
        /// The generated NewCardInfo object.
        /// </returns>
        /// <remarks>
        /// When the V1 AddCard and GetCard APIs are removed, legacy compatibility can be removed from all layers.
        /// </remarks>
        internal static NewCardInfo GenerateLegacyCardInfo(NewCardNumber newCardNumber)
        {
            string number = newCardNumber.Number;
            NewCardInfo result = new NewCardInfo
            {
                Expiration = DateTime.MaxValue,
                Number = number,
                FlightId = newCardNumber.FlightId
            };
            if (String.IsNullOrWhiteSpace(number) == false)
            {
                switch (number[0])
                {
                    case '4':
                        result.CardBrand = "Visa";
                        break;
                    case '5':
                        result.CardBrand = "MasterCard";
                        break;
                    case '3':
                        if (number[1] == '4' || number[1] == '7')
                        {
                            result.CardBrand = "AmericanExpress";
                        }
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Logs the NewCardInfo member of the specified NewCardPayload object with obfuscated fields where required.
        /// </summary>
        /// <param name="newCardInfo">
        /// The NewCardInfo object to obfuscate and log.
        /// </param>
        /// <param name="context">
        /// The context of the current request.
        /// </param>
        internal static void LogObfuscatedNewCardInfo(NewCardInfo newCardInfo,
                                                      CommerceContext context)
        {
            string number = newCardInfo.Number;
            if (String.IsNullOrWhiteSpace(number) == false && number.Length >= 4)
            {
                number = number.Substring(number.Length - 4, 4);
            }

            string logInfo = string.Empty;
            if (newCardInfo != null)
            {
                NewCardInfo obfuscatedNewCardInfo = new NewCardInfo()
                                                    {
                                                        CardBrand = newCardInfo.CardBrand,
                                                        Expiration = newCardInfo.Expiration,
                                                        NameOnCard = newCardInfo.NameOnCard,
                                                        Number = number
                                                    };
                logInfo = General.SerializeJson(obfuscatedNewCardInfo);
            }

            context.Log.Verbose("Obfuscated NewCardInfo:\r\n{0}", logInfo);
        }

        /// <summary>
        /// The request contains a header that helps identify under which reward program's
        /// context the request is coming in.
        /// </summary>
        public static RewardPrograms GetRewardProgramAssociatedWithRequest(HttpRequestMessage requestMessage)
        {
            if (requestMessage == null)
            {
                throw new ArgumentNullException("requestMessage");
            }

            if (requestMessage.ContainsHeader(Constants.FlightIdHeaderName, Constants.FlightIdHeaderValueForEarn))
            {
                return RewardPrograms.EarnBurn;
            }

            return RewardPrograms.CardLinkOffers;
        }
    }
}