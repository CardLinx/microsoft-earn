//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The UserHistoryStorage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.UserServices.Storage.UserHistory
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The UserHistoryStorage interface.
    /// </summary>
    public interface IUserHistoryStorage
    {
        /// <summary>
        /// The save user email entity.
        /// </summary>
        /// <param name="emailEntity">
        /// The email entity.
        /// </param>
        void SaveUserEmailEntity(UserEmailEntity emailEntity);

        /// <summary>
        /// The get user email entities.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// list of entities
        /// </returns>
        IEnumerable<UserEmailEntity> GetUserEmailEntities(Guid userId, int count);
    }
}