//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Interface for priority email jobs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using Users.Dal;

    /// <summary>
    /// Interface for priority email jobs
    /// </summary>
    public interface IPriorityEmailJobHandler
    {
        /// <summary>
        /// Initializes the handler
        /// </summary>
        void Initialize();

        /// <summary>
        /// Executes the priority email job
        /// </summary>
        /// <param name="priorityEmailCargo">Cargo for priority email job</param>
        void Handle(PriorityEmailCargo priorityEmailCargo);
    }
}