//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
// Interface for the user services worker job processor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.Worker.Common
{
    using System.Threading;

    /// <summary>
    /// Interface for the user services worker job processor
    /// </summary>
    public interface IJobProcessor
    {
        /// <summary>
        /// Job processor worker method
        /// </summary>
        /// <param name="ct">
        /// The cancellation token. If ct is null the work will continue for ever otherwise it will continue until a cancel request
        /// </param>
        void DoWork(CancellationToken? ct);
    }
}