//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System;

    /// <summary>
    /// The Probe.aspx page.
    /// </summary>
    /// <remarks>
    /// Probe.aspx is used by MSN-based monitoring to determine whether the machine is current in service. If the file is not
    /// found, the machine is remove from service. Once the file is restored, the machine is returned to service.
    /// </remarks>
    public partial class Probe : System.Web.UI.Page
    {
        /// <summary>
        /// Initializes the page.
        /// </summary>
        /// <param name="sender">
        /// The object that invoked this method.
        /// </param>
        /// <param name="e">
        /// Event arguments sent to this method.
        /// </param>
        protected void Page_Init(object sender,
                                 EventArgs e)
        {
            // Set ViewStateUserKey to prevent one-click attacks.
            ViewStateUserKey = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Loads the page.
        /// </summary>
        /// <param name="sender">
        /// The object that invoked this method.
        /// </param>
        /// <param name="e">
        /// Event arguments sent to this method.
        /// </param>
        protected void Page_Load(object sender,
                                 EventArgs e)
        {
        }
    }
}