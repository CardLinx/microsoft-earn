//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;

    public class SecurityException : Exception
    {
        public SecurityException(string message) : base(message) { }

        public SecurityException(string message, Exception innerException) : base(message, innerException) { }
    }
}