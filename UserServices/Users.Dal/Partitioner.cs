//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the Partitioner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The partitioner.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    internal class Partitioner
    {
        /// <summary>
        /// The max partition count.
        /// </summary>
        internal const int MaxPartitionCount = 1024;

        #region Partition Functions and Methods

        /// <summary>
        /// Assigns partition id to string.
        /// </summary>
        /// <param name="anyString">any string</param>
        /// <returns>partition id</returns>
        /// <exception>Throws exception when anyString is null.</exception>
        internal int PartitionId(string anyString)
        {
            using (var md5Hasher = MD5.Create())
            {
                var hash = md5Hasher.ComputeHash(Encoding.Default.GetBytes(anyString));
                return (hash[0] * 256 * 256) + (hash[1] * 256) + (hash[2] % MaxPartitionCount);
            }
        }

        /// <summary>
        /// Assigns partition id to long.
        /// </summary>
        /// <param name="id">any long integer</param>
        /// <returns>partition id</returns>
        internal int PartitionId(long id)
        {
            ulong key = (ulong)id;
            key += key << 12;
            key ^= key >> 22;
            key += key << 4;
            key ^= key >> 9;
            key += key << 10;
            key ^= key >> 2;
            key += key << 7;
            key ^= key >> 12;
            key += key << 44;
            key ^= key >> 54;
            key += key << 36;
            key ^= key >> 41;
            key += key << 42;
            key ^= key >> 34;
            key += key << 39;
            key ^= key >> 44;
            return (int)(key % MaxPartitionCount);
        }

        /// <summary>
        /// Switches connection to appropriate federation member
        /// </summary>
        /// <param name="context">User Entities context</param>
        /// <param name="partitionId">partition Id</param>
        internal void SwitchConnectionToUsersFederationMember(UsersEntities context, int partitionId)
        {
            string cmd = string.Format(
                "USE FEDERATION Users (PartitionId = {0}) WITH RESET, FILTERING = OFF", partitionId);
            context.Database.ExecuteSqlCommand(cmd);
        }

        #endregion
    }
}