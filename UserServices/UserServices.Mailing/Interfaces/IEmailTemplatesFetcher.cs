//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the IEmailTemplatesFetcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using LoMo.Templating;

    /// <summary>
    /// The TemplateFetcher interface.
    /// </summary>
    /// <typeparam name="TModel">The template mode type. </typeparam>
    public interface IEmailTemplatesFetcher<TModel>
    {
        /// <summary>
        /// The get html body template.
        /// </summary>
        /// <param name="locale">
        /// The locale. If null - the default locale will be used
        /// </param>
        /// <returns>
        /// The <see cref="ITemplate{TModel}"/>.
        /// </returns>
        /// <exception cref="TemplateStoreClientException">Error while retrieving the template from the storage</exception>
        /// <exception cref="TemplateCompileException">Error while compiling the template with the given model</exception>
        ITemplate<TModel> GetHtmlBodyTemplate(Locale locale = null);

        /// <summary>
        /// The get text body template.
        /// </summary>
        /// <param name="locale">
        /// The locale. If null - the default locale will be used
        /// </param>
        /// <returns>
        /// The <see cref="ITemplate{TModel}"/>.
        /// </returns>
        /// <exception cref="TemplateStoreClientException">Error while retrieving the template from the storage</exception>
        /// <exception cref="TemplateCompileException">Error while compiling the template with the given model</exception>
        ITemplate<TModel> GetTextBodyTemplate(Locale locale = null);

        /// <summary>
        /// The get subject template.
        /// </summary>
        /// <param name="locale">
        /// The locale. If null - the default locale will be used
        /// </param>
        /// <returns>
        /// The <see cref="ITemplate{TModel}"/>.
        /// </returns>
        /// <exception cref="TemplateStoreClientException">Error while retrieving the template from the storage</exception>
        /// <exception cref="TemplateCompileException">Error while compiling the template with the given model</exception>
        ITemplate<TModel> GetSubjectTemplate(Locale locale = null);
    }
}