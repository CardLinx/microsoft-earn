//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The TemplateService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.Templating
{
    /// <summary>
    /// The TemplateService interface.
    /// </summary>
    public interface ITemplateService
    {
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
        ITemplate<TModel> GetTemplate<TModel>(string category, string name, Locale locale);
    }
}