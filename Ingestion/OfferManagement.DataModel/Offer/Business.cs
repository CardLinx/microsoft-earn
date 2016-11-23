//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OfferManagement.DataModel
{
    [DataContract]
    public class Business
    {
        /// <summary>
        ///     Gets or sets Name.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "locations")]
        public List<BusinessLocation> Locations { get; set; }

        /// <summary>
        ///     Gets or sets Name.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the web site
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "web_site")]
        public string WebSite { get; set; }


        /// <summary>
        /// Creates shallow copy
        /// </summary>
        /// <returns>The business.</returns>
        public Business ShallowCopy()
        {
            return (Business)this.MemberwiseClone();
        }

    }
}