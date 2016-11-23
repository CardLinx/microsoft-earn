//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Worker
{
    /// <summary>
    /// Specifies the current worker event to be processed.
    /// </summary>
    public enum WorkerEvent
    {
        /// <summary>
        /// Indicates there is no current event needing to be processed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the First Data Extract file is to be processed.
        /// </summary>
        ProcessFirstDataExtract = 1,

        /// <summary>
        /// Indicates that the First Data PTS file is to be processed.
        /// </summary>
        ProcessFirstDataPts = 2,

        /// <summary>
        /// Indicates that the First Data Acknowledgment file is to be processed.
        /// </summary>
        ProcessFirstDataAcknowledgment = 3
    }
}