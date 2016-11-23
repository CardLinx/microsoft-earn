//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The EmailClient interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Email
{
    using System;

    /// <summary>
    /// The EmailClient interface.
    /// </summary>
    public interface IEmailClient
    {
        #region Public Methods and Operators

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="emailInformation">
        /// The email information.
        /// </param>
        /// <param name="correlationId">the correlation id</param>
        void Send(EmailInformation emailInformation, Guid? correlationId);

        #endregion
    }
}