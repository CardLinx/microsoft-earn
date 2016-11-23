//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    /// <summary>
    /// Lists the default event IDs for each log entry type.
    /// </summary>
    public enum DefaultLogEntryEventId
    {
        /// <summary>
        /// Indicates the log entry is for verbose or exhaustive logging purposes.
        /// </summary>
        Verbose = 0,

        /// <summary>
        /// Indicates the log entry is to record the completion of an API call.
        /// </summary>
        CallCompletion = 42,

        /// <summary>
        /// Indicates the log entry is for informational purposes.
        /// </summary>
        Information = 100,

        /// <summary>
        /// Indicates the log entry is a warning.
        /// </summary>
        Warning = 200,

        /// <summary>
        /// Indicates the log entry is a warning generated due to a partner error.
        /// </summary>
        PartnerErrorWarning = 300,

        /// <summary>
        /// Indicates the log entry is an error.
        /// </summary>
        Error = 400,

        /// <summary>
        /// Indicates the log entry is a critical error.
        /// </summary>
        Critical = 500
    }
}