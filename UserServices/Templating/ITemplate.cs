//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Template interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.Templating
{
    /// <summary>
    /// The Template interface.
    /// </summary>
    /// <typeparam name="TModel">
    /// the template model type
    /// </typeparam>
    public interface ITemplate<TModel>
    {
        /// <summary>
        /// Render the template with the given model
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="TemplateRenderException">error while rendering the template</exception>
        string Render(TModel model);
    }
}