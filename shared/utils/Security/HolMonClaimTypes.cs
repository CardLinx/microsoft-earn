//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System.Security.Claims;

    public static class HolMonClaimTypes
    {
        public const string AuthenticationScheme = "AuthScheme";
        public const string TokenAudience = "Audience";
        public const string TokenIssuer = "Issuer";
        public const string Role = ClaimTypes.Role;
    }
}