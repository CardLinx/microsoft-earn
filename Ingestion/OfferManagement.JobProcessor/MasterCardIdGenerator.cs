//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using OfferManagement.DataModel;
using System;
using System.Text;

namespace OfferManagement.JobProcessor
{
    public static class MasterCardIdGenerator
    {
        public static string GetUniqueId(Provider provider, Merchant merchant, string merchantSource)
        {
            // We stick a unique identifier for a merchant when sending the data back to mastercard. For this, we are using
            // one of the pass through fields of length 30. The logic to generate a unique id is as follows
            // 1. First character depicts the merchant source. For nationals, we get the data from MC and so it's "P"
            //    For locals, it depicts the first char from the offer provider
            // 2. First 5 characters comes from the provider name after removing non alphabets
            //    If we fall short of the required length of 5, we fill the remaining places with random characters
            // 3. Next 2 Characters comes from the merchant city. If city is not available, fill it with "NA".             
            // 4. Next 22 characters comes from the base64 encoded version of guid. Here again, non letters are removed
            //    and filled with random characters

            string providerNamePart = GetProviderNamePart(provider);
            providerNamePart = providerNamePart.Substring(0, 5);
            string merchantCityPart = GetCityPart(merchant);
            string base64EncodedGuid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            string guid = base64EncodedGuid.Substring(0, 22);
            for (int j = 0; j < guid.Length; j++)
            {
                if (!char.IsLetter(guid[j]))
                {
                    Random _random = new Random((Environment.MachineName + DateTime.Now.Ticks).GetHashCode());
                    char randomChar = Convert.ToChar('A' + _random.Next(26));
                    guid = guid.Replace(guid[j], randomChar);
                }
            }

            return $"{merchantSource}{providerNamePart}{merchantCityPart}{guid}";
        }

        private static string GetProviderNamePart(Provider provider)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < provider.Name.Length; i++)
            {
                if (char.IsLetter(provider.Name[i]))
                {
                    sb.Append(provider.Name[i]);
                }
            }

            int requiredLength = 5;
            int currentLength = sb.Length;
            if (currentLength < 5)
            {
                for (int j = 0; j < requiredLength - currentLength; j++)
                {
                    sb.Append(GenerateRandomChar());
                }
            }

            return sb.ToString();
        }

        private static string GetCityPart(Merchant merchant)
        {
            string merchantCity = merchant.Location?.City;
            string merchantCityPart = !string.IsNullOrWhiteSpace(merchantCity) ? merchantCity.Substring(0, 2) : "NA";

            return merchantCityPart;
        }

        private static char GenerateRandomChar()
        {
            Random _random = new Random((Environment.MachineName + DateTime.Now.Ticks).GetHashCode());
            char randomChar = Convert.ToChar('A' + _random.Next(26));

            return randomChar;
        }

    }
}