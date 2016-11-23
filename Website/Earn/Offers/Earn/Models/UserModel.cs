//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Earn.Offers.Earn.Models
{
    public class UserModel
    {
        public UserModel(ClaimsIdentity claimsidentity)
        {
            if (claimsidentity != null && claimsidentity.IsAuthenticated)
            {
                this.Authenticated = true;
                Claim emailClaim = claimsidentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
                {
                    this.UserEmail = emailClaim.Value;
                }

                Claim nameClaim = claimsidentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
                if (nameClaim != null && !string.IsNullOrEmpty(nameClaim.Value))
                {
                    this.Name = nameClaim.Value;
                }

                Claim sidClaim = claimsidentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
                if (sidClaim != null && !string.IsNullOrEmpty(sidClaim.Value))
                {
                    this.UserId = sidClaim.Value;
                }

                Claim anidClaim = claimsidentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Anonymous);
                if (anidClaim != null && !string.IsNullOrEmpty(anidClaim.Value))
                {
                    this.Anid = anidClaim.Value;
                }
            }
        }

        public string UserId
        {
            get;
            set;
        }

        public string Anid
        {
            get;
            set;
        }

        public bool Authenticated
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string UserEmail
        {
            get;
            set;
        }
    }
}