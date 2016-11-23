//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The UserServicesClient interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System;

    /// <summary>
    /// The UserServicesClient interface.
    /// </summary>
    public interface IUserServicesClient
    {
        /// <summary>
        /// The send email.
        /// </summary>
        /// <param name="correlationId"> The correlation id. </param>
        /// <param name="request"> The request. </param>
        /// <param name="requestTimeout"> The request Timeout./// </param>
        void SendEmail(Guid correlationId, SendEmailRequest request, TimeSpan? requestTimeout);
    }
}