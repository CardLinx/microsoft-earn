//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The EmailRenderingClient interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    /// <summary>
    /// The EmailRenderingClient interface.
    /// </summary>
    public interface IEmailRenderingClient<T>
    {
        string EmailRenderingServiceUrl { get; set; }

        /// <summary>
        /// Posts a request to the Template service and returns the Html
        /// </summary>
        /// <param name="model">
        /// The model which is the payload for the Post request to template service
        /// </param>
        /// <returns>
        /// Html Content
        /// </returns>
        string RenderHtml(T model);

        /// <summary>
        /// Returns the Html by doing a Get request to the Template Service
        /// </summary>
        /// <returns>Html Content</returns>
        string RenderHtml();

    }
}