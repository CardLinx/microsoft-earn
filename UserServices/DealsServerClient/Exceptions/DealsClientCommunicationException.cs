//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The deals client communication exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DealsServerClient.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The deals client communication exception.
    /// </summary>
    public class DealsClientCommunicationException : DealsClientException
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DealsClientCommunicationException"/> class. 
        /// </summary>
        public DealsClientCommunicationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DealsClientCommunicationException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The message that describes the error. 
        /// </param>
        public DealsClientCommunicationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DealsClientCommunicationException"/> class. 
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. 
        /// </param>
        public DealsClientCommunicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DealsClientCommunicationException"/> class. 
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. 
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null. 
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). 
        /// </exception>
        protected DealsClientCommunicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}