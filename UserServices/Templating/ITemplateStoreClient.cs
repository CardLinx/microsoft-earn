//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the ITemplateStoreClient interface .
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.Templating
{
    /// <summary>
    /// The TemplateFetcher interface.
    /// </summary>
    public interface ITemplateStoreClient
    {
        /// <summary>
        /// Downloading template from the store.
        /// </summary>
        /// <param name="category">
        /// The template category.
        /// </param>
        /// <param name="name">
        /// The template name.
        /// </param>
        /// <param name="locale">
        /// The template locale.
        /// </param>
        /// <returns>The template.</returns>
        /// <exception cref="TemplateStoreClientException">Error while downloading the template from the store. </exception>
        string DownloadTemplate(string category, string name, Locale locale);

        /// <summary>
        /// Uploading template to the store.
        /// </summary>
        /// <param name="category">
        /// The template category.
        /// </param>
        /// <param name="name">
        /// The template name.
        /// </param>
        /// <param name="locale">
        /// The template locale.
        /// </param>
        /// <param name="template">
        /// The template content.
        /// </param>
        /// <exception cref="TemplateStoreClientException">Error while uploading the template to the store. </exception>
        void UploadTemplate(string category, string name, Locale locale, string template);


        /// <summary>
        /// Deletes template from the store.
        /// </summary>
        /// <param name="category">
        /// The template category.
        /// </param>
        /// <param name="name">
        /// The template name.
        /// </param>
        /// <param name="locale">
        /// The template locale.
        /// </param>
        /// <exception cref="TemplateStoreClientException">Error while deleting the template from the store. </exception>
        void DeleteTemplate(string category, string name, Locale locale);

        /// <summary>
        /// Deletes all the templates of category from the store.
        /// </summary>
        /// <param name="category">
        /// The template category.
        /// </param>
        /// <exception cref="TemplateStoreClientException">Error while deleting the templates from the store. </exception>
        void DeleteCategoryTemplates(string category);
    }
}