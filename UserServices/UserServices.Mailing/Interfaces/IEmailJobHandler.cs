//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Interface for email job handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    /// <summary>
    /// Interface for email job handler
    /// </summary>
    public interface IEmailJobHandler
    {
        /// <summary>
        /// Initializes the Handler
        /// </summary>
        void Initialize();

        /// <summary>
        /// Executes the email job
        /// </summary>
        /// <param name="emailCargo">Cargo for the email job</param>
        void Handle(EmailCargo emailCargo);
    }
}