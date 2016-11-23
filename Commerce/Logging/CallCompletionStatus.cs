//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    /// <summary>
    /// Specifies call status at the completion of the call.
    /// </summary>
    public enum CallCompletionStatus
    {
        /// <summary>
        /// Indicates the call completed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// Indicates the call completed successfully, but warnings were generated.
        /// </summary>
        SuccessWithWarnings,

        /// <summary>
        /// Indicates the call failed.
        /// </summary>
        Error
    }
}