//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Commerce.DataContracts.Extensions;
using Lomo.Commerce.VisaClient;
using services.visa.com.realtime.realtimeservice.datacontracts.v6;
using System.Linq;
using Visa.Proxy;

namespace Commerce.VisaClient
{
    public class VisaErrorUtility
    {
        public static readonly VisaErrorUtility Instance = new VisaErrorUtility();

        public ResultCode GetResultCode(Base_Response response, string[] errors)
        {
            var resultCode = ResultCode.UnknownError;

            if (errors == null)
            {
                errors = VisaCallErrorConstants.VisaErrorToResultCodeMapping.Keys.ToArray();
            }

            foreach (var error in errors)
            {
                if (response.ContainsError(error))
                {
                    if (VisaCallErrorConstants.VisaErrorToResultCodeMapping.ContainsKey(error))
                    {
                        resultCode = VisaCallErrorConstants.VisaErrorToResultCodeMapping[error];
                        break;
                    }
                }
            }

            return resultCode;
        }
    }
}