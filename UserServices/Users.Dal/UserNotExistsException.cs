//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Users.Dal
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The user not exists exception.
    /// </summary>
    public class UserNotExistsException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserNotExistsException" /> class.
        /// </summary>
        public UserNotExistsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotExistsException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public UserNotExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotExistsException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public UserNotExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotExistsException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected UserNotExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}