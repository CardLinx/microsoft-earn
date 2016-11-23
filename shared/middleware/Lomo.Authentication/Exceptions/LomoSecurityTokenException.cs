//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authentication.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// LomoSecurityTokenException exception class.
    /// </summary>
    public class LomoSecurityTokenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LomoSecurityTokenException"/> class.
        /// </summary>
        /// <param name="message">The exception message string.</param>
        public LomoSecurityTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LomoSecurityTokenException"/> class.
        /// </summary>
        /// <param name="message">The exception message string.</param>
        /// <param name="innerException">The inner exception.</param>
        public LomoSecurityTokenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}