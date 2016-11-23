//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using services.visa.com.realtime.realtimeservice.datacontracts.v6;
using System;
using System.Linq;
using System.Text;

namespace Visa.Proxy
{
    public static class BaseResponseExtensions
    {
        public static string GetErrorInfoDetail(this Base_Response response)
        {
            var sb = new StringBuilder();

            if (response != null)
            {
                var referenceId = string.Format("VisaReferenceId:{0}", response.ReferenceId);
                sb.Append(referenceId);
                sb.AppendLine();

                var error = response.GetError();
                var errMsg = string.Format("Error:{0}", error);
                sb.Append(errMsg);
                sb.AppendLine();

                var info = response.GetInfo();
                var infoMsg = string.Format("Info:{0}", info);
                sb.Append(infoMsg);
                sb.AppendLine();
            }

            var errorInfoDetail = sb.ToString();
            return errorInfoDetail;
        }

        public static string GetError(this Base_Response response)
        {
            var sb = new StringBuilder();

            if (response != null)
            {
                if (response.Errors != null && response.Errors.Any())
                {
                    foreach (var responseError in response.Errors)
                    {
                        var errMsg = string.Format("{0} {1}", responseError.ErrorCode, responseError.ErrorMessage);
                        sb.Append(errMsg);
                        sb.AppendLine();
                    }
                }
            }

            var errorInfoDetail = sb.ToString();
            return errorInfoDetail;
        }

        public static string GetInfo(this Base_Response response)
        {
            var sb = new StringBuilder();

            if (response != null)
            {
                if (response.Information != null && response.Information.Any())
                {
                    foreach (var information in response.Information)
                    {
                        var infoMsg = string.Format("{0} {1}", information.Code, information.Description);
                        sb.Append(infoMsg);
                        sb.AppendLine();
                    }
                }
            }

            var errorInfoDetail = sb.ToString();
            return errorInfoDetail;
        }

        public static bool HasError(this Base_Response response)
        {
            return response != null && response.Errors != null && response.Errors.Any();
        }

        public static bool ContainsError(this Base_Response response, string errorCode)
        {
            return response.HasError() && response.Errors.Any(e => string.Equals(e.ErrorCode, errorCode, StringComparison.OrdinalIgnoreCase));
        }
    }
}