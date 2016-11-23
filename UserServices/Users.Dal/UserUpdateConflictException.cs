//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The user update exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The user update exception.
    /// </summary>
    public class UserUpdateConflictException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpdateConflictException"/> class.
        /// </summary>
        public UserUpdateConflictException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpdateConflictException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public UserUpdateConflictException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpdateConflictException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public UserUpdateConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpdateConflictException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected UserUpdateConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}