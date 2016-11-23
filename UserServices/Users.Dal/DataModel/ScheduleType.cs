//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The Merchant Preferences
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal.DataModel
{
    /// <summary>
    /// Schedule for receiving merchant email
    /// </summary>
    public enum ScheduleType
    {
        /// <summary>
        /// The Daily 
        /// </summary>
        Daily = 0,

        /// <summary>
        /// The weekly
        /// </summary>
        Weekly = 1,

        /// <summary>
        /// The monthly
        /// </summary>
        Monthly = 2
    }
}