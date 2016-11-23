//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerCommon
{
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Processes First Data Extract files.
    /// </summary>
    public interface ISettlementFileProcessor
    {
        /// <summary>
        /// Processes the Extract file.
        /// </summary>
        Task Process();

        /// <summary>
        /// The ID of the task thread in which this object is operating.
        /// </summary>
        int ThreadId { get; set; }
    }
}