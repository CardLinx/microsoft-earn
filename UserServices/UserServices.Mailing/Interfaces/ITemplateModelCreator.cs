//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the ITemplateModelCreator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    /// <summary>
    /// The TemplateModelCreator interface.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of the model object
    /// </typeparam>
    public interface ITemplateModelCreator<out TModel>
    {
        /// <summary>
        /// The generate model.
        /// </summary>
        /// <param name="job">
        /// The job.
        /// </param>
        /// <param name="deals">
        /// The deals.
        /// </param>
        /// <returns>
        /// The <see cref="TModel"/>.
        /// </returns>
        TModel GenerateModel(EmailTemplateData emailModelData);
    }
}