//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The EmailContentCreatorFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    /// <summary>
    /// The EmailContentCreatorFactory interface.
    /// </summary>
    public interface IEmailContentCreatorFactory
    {
        /// <summary>
        /// The get content creator.
        /// </summary>
        /// <param name="emailCargo">
        /// The job.
        /// </param>
        /// <returns>
        /// The <see cref="IEmailContentCreator"/>.
        /// </returns>
        IEmailContentCreator GetContentCreator(EmailCargo emailCargo);
    }
}