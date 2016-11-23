//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earn.Models
{
    public enum DeviceType
    {
        /// <summary>
        /// Unknown device type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Mobile device type.
        /// </summary>
        Mobile = 1,

        /// <summary>
        /// Tablet device type.
        /// </summary>
        Tablet = 2,

        /// <summary>
        /// Small desktop device.
        /// </summary>
        SmallDekstop = 3,

        /// <summary>
        /// Desktop device
        /// </summary>
        Desktop = 4
    }
}