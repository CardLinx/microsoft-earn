//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The template service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.Templating
{
    /// <summary>
    /// The template service.
    /// </summary>
    public class TemplateService : ITemplateService
    {
        #region Data Members

        /// <summary>
        /// The template store client.
        /// </summary>
        private readonly ITemplateStoreClient templateStoreClient;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateService"/> class.
        /// </summary>
        public TemplateService()
        {
            this.templateStoreClient = new TemplateBlobStoreClient();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateService"/> class.
        /// </summary>
        /// <param name="storeClient">
        /// The store client.
        /// </param>
        public TemplateService(ITemplateStoreClient storeClient)
        {
            this.templateStoreClient = storeClient;
        }

        #endregion

        #region ITemplateService Imp

        /// <summary>
        /// Returns the requested template
        /// </summary>
        /// <param name="category"> The template category. </param>
        /// <param name="name"> The template name. </param>
        /// <param name="locale"> The template locale. </param>
        /// <typeparam name="TModel"> The template input model type </typeparam>
        /// <returns> The <see cref="ITemplate{TModel}"/>.</returns>
        /// <exception cref="TemplateStoreClientException">Error while retrieving the template from the storage</exception>
        /// <exception cref="TemplateCompileException">Error while compiling the template with the given model</exception>
        public ITemplate<TModel> GetTemplate<TModel>(string category, string name, Locale locale)
        {
            var templateStr = this.templateStoreClient.DownloadTemplate(category, name, locale);
            return new Template<TModel>(templateStr);
        }

        #endregion
    }
}