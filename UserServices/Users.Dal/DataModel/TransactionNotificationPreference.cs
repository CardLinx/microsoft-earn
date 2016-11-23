//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// Represents the medium through which transaction notifications are delivered to the user
// ------------------------------------------------------------------------------------------------------------------------------

namespace Users.Dal.DataModel
{
    using System;

    /// <summary>
    /// Represents the medium through which transaction notifications are delivered to the user
    /// </summary>
    [Flags]
    public enum TransactionNotificationPreference
    {
        /// <summary>
        /// No notifications should be delivered
        /// </summary>
        None = 0,

        /// <summary>
        /// Deliver notifications via email
        /// </summary>
        Email = 1,

        /// <summary>
        /// Deliver notifications via phone/sms
        /// </summary>
        Phone = 2
    }
}