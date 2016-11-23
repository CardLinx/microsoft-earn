//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Earn.DataContract
{
    [DataContract]
    public class Deal
    {
        /// <summary>
        ///     Gets or sets Business.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "business")]
        public Business Business { get; set; }

        /// <summary>
        ///     Gets or sets dealrank.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_rank")]
        public int DealRank { get; set; }

        /// <summary>
        ///     Gets or sets Description.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "description")]
        public string Description { get; set; }


        /// <summary>
        ///     Gets or sets Id.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets ImageUrl.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        ///     Gets or sets Images.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_images")]
        public List<DealImage> Images { get; set; }

        /// <summary>
        ///     Gets or sets the keywords.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "keywords")]
        public List<string> Keywords { get; set; }

        /// <summary>
        ///     Gets or sets Title.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets TransactionUrl.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "transaction_url")]
        public string TransactionUrl { get; set; }


        /// <summary>
        ///     Gets or sets provider name.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "provider_name")]
        public string ProviderName { get; set; }

        /// <summary>
        /// Creates shallow copy
        /// </summary>
        /// <returns>The business.</returns>
        public Deal ShallowCopy()
        {
            return (Deal)this.MemberwiseClone();
        }
    }
}