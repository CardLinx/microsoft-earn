//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the Template type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.Templating
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using RazorEngine;

    /// <summary>
    /// <see cref="ITemplate{TModel}"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1618:GenericTypeParametersMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    internal class Template<TModel> : ITemplate<TModel>
    {
        #region Data Members

        /// <summary>
        /// The templateContent.
        /// </summary>
        private readonly string templateContent;

        /// <summary>
        /// The templateContent cache key.
        /// </summary>
        private readonly string templateCacheKey;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Template{TModel}"/> class.
        /// </summary>
        /// <param name="templateContent">
        /// The templateContent.
        /// </param>
        /// <exception cref="TemplateCompileException">Template compilation error</exception>
        internal Template(string templateContent)
        {
            this.templateContent = templateContent;
            this.templateCacheKey = Guid.NewGuid().ToString();
            try
            {
                Razor.Compile<TModel>(this.templateContent, this.templateCacheKey);
            }
            catch (Exception exception)
            {
                throw new TemplateCompileException(string.Format("Error while Compiling template. Model Type: {0}", typeof(TModel)), exception);
            }
        }

        #endregion

        #region ITemplate Imp

        /// <summary>
        /// <see cref="ITemplate{TModel}"/>
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("Style" + "Cop.CSharp.DocumentationRules", "SA1615:ElementReturnValueMustBeDocumented", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public string Render(TModel model)
        {
            try
            {
                return Razor.Parse(this.templateContent, model, this.templateCacheKey);
            }
            catch (Exception exception)
            {
                throw new TemplateRenderException(string.Format("Error while rendering the template. Model Type: {0}", typeof(TModel)), exception);
            }
        }

        #endregion
    }
}