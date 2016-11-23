//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the EmailTemplatesFetcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using LoMo.Templating;

    /// <summary>
    /// The email templates fetcher.
    /// </summary>
    /// <typeparam name="TModel"> The model. </typeparam>
    public class EmailTemplatesFetcher<TModel> : IEmailTemplatesFetcher<TModel>
    {
        #region Consts

        /// <summary>
        /// The html body template name.
        /// </summary>
        internal const string HtmlBodyTemplateName = "body_html.cshtml";

        /// <summary>
        /// The text body template name.
        /// </summary>
        internal const string TextBodyTemplateName = "body_text.cshtml";

        /// <summary>
        /// The subject template name.
        /// </summary>
        internal const string SubjectTemplateName = "subject.cshtml";
        
        #endregion

        #region Data Members

        /// <summary>
        /// The template service.
        /// </summary>
        private readonly ITemplateService templateService;

        /// <summary>
        /// The templates identifier.
        /// </summary>
        private readonly string templatesIdentifier;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplatesFetcher{TModel}"/> class.
        /// </summary>
        /// <param name="templateService">
        /// The template service.
        /// </param>
        /// <param name="templatesIdentifier">
        /// The templates identifier.
        /// </param>
        public EmailTemplatesFetcher(ITemplateService templateService, string templatesIdentifier)
        {
            this.templateService = templateService;
            this.templatesIdentifier = templatesIdentifier;
        }

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
        public ITemplate<TModel> GetHtmlBodyTemplate(Locale locale = null)
        {
            return this.templateService.GetTemplate<TModel>(this.templatesIdentifier, HtmlBodyTemplateName, locale);
        }
        
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
        public ITemplate<TModel> GetTextBodyTemplate(Locale locale = null)
        {
            return this.templateService.GetTemplate<TModel>(this.templatesIdentifier, TextBodyTemplateName, locale);
        }

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
        public ITemplate<TModel> GetSubjectTemplate(Locale locale = null)
        {
            return this.templateService.GetTemplate<TModel>(this.templatesIdentifier, SubjectTemplateName, locale);
        }
    }
}