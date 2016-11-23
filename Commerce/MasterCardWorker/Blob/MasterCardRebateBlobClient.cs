//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Contains methods to upload rebate files to the blob store and to mark them as having been uploaded to MasterCard.
    /// </summary>
    public class MasterCardRebateBlobClient : MasterCardBlobClient
    {
        /// <summary>
        /// Initializes a new instance of the MasterCardBlobClient class.
        /// </summary>
        /// <remarks>
        /// This constructor is meant for test use only.
        /// </remarks>
        public MasterCardRebateBlobClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MasterCardBlobClient class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object within which to place log entries.
        /// </param>
        public MasterCardRebateBlobClient(CommerceLog log)
            : base(RebateFolderName, RebateFileTypeDescription, log)
        {
        }

        /// <summary>
        /// Gets the decoration prepended to the name of files that have yet to be uploaded to MasterCard.
        /// </summary>
        protected override string UnprocessedDecoration
        {
            get
            {
                return Decoration;
            }
        }

        /// <summary>
        /// The name of the folder in which rebate files will be uploaded.
        /// </summary>
        private const string RebateFolderName = "mastercard-rebatefiles";

        /// <summary>
        /// The description for MasterCard rebate files.
        /// </summary>
        private const string RebateFileTypeDescription = "MasterCard Rebate";

        /// <summary>
        /// The decoration prepended to the name of files that have yet to be uploaded to MasterCard.
        /// </summary>
        private const string Decoration = "ToBeUploaded";
    }
}