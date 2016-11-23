//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Respresents Amex Offer Registration Response File
    /// </summary>
    public class OfferRegistrationResponseFile
    {
        /// <summary>
        /// Gets or sets the Response File's Header
        /// </summary>
        public OfferRegistrationResponseHeader Header { get; set; }

        /// <summary>
        /// Gets Response File's records
        /// </summary>
        public Collection<OfferRegistrationResponseDetail> ResponseRecords
        {
            get
            {
                return responseRecords;
            }
        }
        private Collection<OfferRegistrationResponseDetail> responseRecords = new Collection<OfferRegistrationResponseDetail>(); 

        /// <summary>
        /// Gets or sets the Response File's Trailer
        /// </summary>
        public OfferRegistrationResponseTrailer Trailer { get; set; }
    }
}