//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Threading.Tasks;
using Commerce.VisaClient;
using Lomo.Commerce.DataContracts.Extensions;
using Visa.Proxy;

namespace Lomo.Commerce.VisaClient
{

    /// <summary>
    ///  Some misc operations 
    /// </summary>
    public class VisaOperations
    {
        /// <summary>
        ///  Unenroll an user so that all his stuff, especially his card record is cleaned in Visa side. 
        /// </summary>
        /// <param name="userKey"></param>
        public static async Task<ResultCode> UnenrollUser(string userKey)
        {
            //ServicePointManager.ServerCertificateValidationCallback +=
            //    (sender, cert, chain, sslPolicyErrors) => true;

            //var client = VisaRtmClientManager.Instance;

            ResultCode result = ResultCode.None;

            var unenrollmentRequst = VisaRtmDataManager.GetUnenrollRequest(userKey, VisaConstants.CommunityName);

            var response = await new VisaInvoker().RemoveUser(unenrollmentRequst, false).ConfigureAwait(false);

            if (response.Success)
                result = ResultCode.Success;
            else if (response.HasError())
            {
                result = VisaErrorUtility.Instance.GetResultCode(response, null);
            }

            return result;
        }
    }
}