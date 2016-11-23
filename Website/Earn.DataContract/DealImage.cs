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
    public class DealImage
    {
        #region Enums

        /// <summary>
        /// The image status type.
        /// </summary>
        public enum ImageStatusType
        {
            /// <summary>
            /// The unknown.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// The good image.
            /// </summary>
            GoodImage = 1,

            /// <summary>
            /// The default image being used.
            /// </summary>
            DefaultImageBeingUsed = 2,

            /// <summary>
            /// The invalid image.
            /// </summary>
            InvalidImage = 3
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///    Gets or sets the dimension: height
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "image_height")]
        public int Height { get; set; }

        /// <summary>
        ///  Gets or sets the Image status
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "image_status")]
        public ImageStatusType ImageStatus { get; set; }

        /// <summary>
        ///   Gets or sets the image url
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        ///  Gets or sets the rank to decide the best image.  Rank = 0 - lowest rank.  Rank = 1 - highest rank
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "rank")]
        public float Rank { get; set; }

        /// <summary>
        ///  Gets or sets the dimension: width
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "image_width")]
        public int Width { get; set; }

        #endregion
    }
}