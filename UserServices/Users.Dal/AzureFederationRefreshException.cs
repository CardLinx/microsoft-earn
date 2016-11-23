//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the AzureFederationRefreshException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The azure federation refresh exception.
    /// </summary>
    [Serializable]
    public class AzureFederationRefreshException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureFederationRefreshException"/> class.
        /// </summary>
        public AzureFederationRefreshException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureFederationRefreshException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public AzureFederationRefreshException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureFederationRefreshException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public AzureFederationRefreshException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureFederationRefreshException"/> class.
        /// </summary>
        /// <param name="serializationInfo">
        /// The serializationInfo.
        /// </param>
        /// <param name="streamingContext">
        /// The streamingContext.
        /// </param>
        public AzureFederationRefreshException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}