//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Data Model to reprsent the user profile information
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProfileService.DataContract
{
    using System.Runtime.Serialization;

    /// <summary>
    ///  Data Model to reprsent the user profile information
    /// </summary>
    [DataContract]
    public class UserDemographics
    {
        //Gender of the user - Male, Female or Unknown
        [DataMember]
        public string Gender { get; set; }

        //Age of the user
        [DataMember]
        public int Age { get; set; }

        //Flag to indicate whether the user has opted out of targeted ads - Need to respect this bit before pulling in the targeted deal
        [DataMember]
        public bool? OptOut { get; set; }
    }

}