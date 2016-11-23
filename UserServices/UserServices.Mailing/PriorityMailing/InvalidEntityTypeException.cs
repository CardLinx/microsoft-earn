//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Represents an error when the confirmation email job message has an invalid entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;

    /// <summary>
    /// Represents an error when the confirmation email job message has an invalid entity
    /// </summary>
    public class InvalidEntityTypeException : Exception
    {
        public InvalidEntityTypeException()
        {
            
        }

        public InvalidEntityTypeException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}