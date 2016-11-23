//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The confirmetion link gererator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System;

    using Users.Dal.DataModel;

    /// <summary>
    /// The confirmation link generator.
    /// </summary>
    public class ConfirmationLinkGenerator
    {
        /// <summary>
        /// The base uri integration.
        /// </summary>
        private static readonly Uri BaseUriIntegration = new Uri("https://ssl.bing-exp.com/offers/proxy/preferences/confirm/");

        /// <summary>
        /// The base uri prod.
        /// </summary>
        private static readonly Uri BaseUriProd = new Uri("https://bing.com/offers/proxy/preferences/confirm/");

        /// <summary> The the confirmation link
        /// </summary>
        /// <param name="userIdHash"> The user id hash. </param>
        /// <param name="confirmationCode"> The confirmation code. </param>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="environment"> The environment. </param>
        /// <returns>
        /// The confirmation link
        /// </returns>
        /// <exception cref="NotSupportedException"> entity type isn't EmailAddress or AccountLink or environment isn't Integration or Production </exception>
        public static Uri GetLink(string userIdHash, int confirmationCode, EntityType entityType, EnvironmentType environment)
        {
            Uri baseUri;
            string method;
            switch (environment)
            {
                case EnvironmentType.Int:
                    baseUri = BaseUriIntegration;
                    break;
                case EnvironmentType.Prod:
                    baseUri = BaseUriProd;
                    break;
                default:
                    throw new NotSupportedException(string.Format("The only supported envrironments are Prod and Int. {0} isn't supported", environment));
            }

            switch (entityType)
            {
                case EntityType.AuthenticatedEmailAddress:
                    method = "email";
                    break;
                case EntityType.UnAuthenticatedEmailAddress:
                    method = "coemail";
                    break;
                case EntityType.AccountLink:
                    method = "alink";
                    break;
                default:
                    throw new NotSupportedException(string.Format("The only supported entities are EmailAddress and AccountLink. {0} isn't supported", entityType));
            }

            string relativeUri = string.Format("{0}?uh={1}&c={2}", method, userIdHash, confirmationCode);
            return new Uri(baseUri, relativeUri);
        }
    }
}