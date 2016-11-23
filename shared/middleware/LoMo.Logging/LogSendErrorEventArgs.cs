//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the LogSendErrorEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.Logging
{
    using System;

    /// <summary>
    /// Log send error event arguments
    /// </summary>
    public class LogSendErrorEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the <see cref="LogSendErrorEventArgs" /> class
        /// </summary>
        /// <param name="error">the error</param>
        public LogSendErrorEventArgs(string error)
        {
            this.Error = error;
        }

        #endregion

        #region Data Members

        /// <summary>
        /// Gets the error details
        /// </summary>
        public string Error { get; private set; }

        #endregion
    }
}