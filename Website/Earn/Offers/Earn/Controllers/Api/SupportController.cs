//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Attributes;
using Earn.Offers.Earn.Models;
using Earn.Offers.Earn.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Earn.Offers.Earn.Controllers.Api
{
    public class SupportController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]SupportRequestModel model)
        {
            HttpResponseMessage responseMessage = null;
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.AssistanceWith))
            {
                responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                responseMessage.Content = new StringContent("Enter all the required fields.");
                return responseMessage;
            }

            if (!User.Identity.IsAuthenticated)
            {
                responseMessage = new HttpResponseMessage(HttpStatusCode.Forbidden);
                responseMessage.Content = new StringContent("You need to log in to perform this operation.");
                return responseMessage;
            }

            ClaimsPrincipal cp = User as ClaimsPrincipal;
            Claim nameClaim = cp.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault();
            string name = string.Empty;
            if (nameClaim != null)
            {
                name = nameClaim.Value;
            }

            EmailInfo info = new EmailInfo
            {
                Category = "EarnByMicrosoft Support",
                From = model.Email,
                FromDisplayName = name,
                Subject = model.AssistanceWith,
                HtmlBody = model.ToString(),
                To = new List<string>
                {
                    "earnsupport@microsoft.com"
                }
            };

            try
            {
                await EmailService.SendEmail(info);
                responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = new StringContent("Your request has been submitted successfully");
                return await Task.FromResult<HttpResponseMessage>(responseMessage);
            }
            catch (Exception e)
            {
            }

            responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            responseMessage.Content = new StringContent("An error occured sending the request. Retry later. If the issue persists, email earnsupport@microsoft.com");
            return await Task.FromResult<HttpResponseMessage>(responseMessage);
        }
    }
}