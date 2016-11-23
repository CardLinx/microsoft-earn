//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    /// <summary>
    /// List of entities that can issue SWT tokens 
    /// that our security module will recognize.
    /// </summary>
    public enum TokenIssuer
    {
        /// <summary>
        /// The token issuer is not known.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The token was issued by the Macallan UX
        /// </summary>
        MacallanUx = 1,

        /// <summary>
        /// The token was issued by the CopyToEpiphany program.
        /// </summary>
        CopyToEpiphany = 2,

        /// <summary>
        /// The token was issued by the TestUI program.
        /// </summary>
        TestUx = 3,

        /// <summary>
        /// The token was issued by the Earn Dashboard.
        /// </summary>
        EarnDashboard = 4,
    }
}