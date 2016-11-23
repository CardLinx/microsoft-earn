//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    /// <summary>
    /// Constants related to MasterCard FTP Calls
    /// </summary>
    public static class MasterCardFtpConstants
    {
        /// <summary>
        /// Property Name for Flag to Run File Processing Job or not
        /// </summary>
        public static readonly string RunMasterCardFileProcessingJobsPropertyName = "Lomo.Commerce.RunMasterCardFileProcessingJobs";

        /// <summary>
        /// MasterCard's FTP URI authority.
        /// </summary>
        public static readonly string AddressAuthority = "Lomo.Commerce.MasterCard.Ftp.Authority";

        /// <summary>
        /// MasterCard's FTP username.
        /// </summary>
        public static readonly string UserName = "Lomo.Commerce.MasterCard.Ftp.UserName";

        /// <summary>
        /// MasterCard's FTP password
        /// </summary>
        public static readonly string Password = "Lomo.Commerce.MasterCard.Ftp.Password";

        /// <summary>
        /// MasterCard's filtering FTP Directory
        /// </summary>
        public static readonly string FilteringDirectory = "Lomo.Commerce.MasterCard.Ftp.FilteringDirectory";

        /// <summary>
        /// MasterCard's clearing FTP Directory
        /// </summary>
        public static readonly string ClearingDirectory = "Lomo.Commerce.MasterCard.Ftp.ClearingDirectory";

        /// <summary>
        /// MasterCard's rebate FTP Directory
        /// </summary>
        public static readonly string RebateDirectory = "Lomo.Commerce.MasterCard.Ftp.RebateDirectory";

        /// <summary>
        /// MasterCard's rebate confirmation FTP Directory
        /// </summary>
        public static readonly string RebateConfirmationDirectory = "Lomo.Commerce.MasterCard.Ftp.RebateConfirmationDirectory";

        /// <summary>
        /// The substring that will be found in relevant files during clearing file downloads.
        /// </summary>
        public static readonly string ClearingRelevantFileNameSubstring = "Lomo.Commerce.MasterCard.Ftp.ClearingRelevantFileNameSubstring";

        /// <summary>
        /// The substring that will be found in relevant files during rebate confirmation file downloads.
        /// </summary>
        public static readonly string RebateConfirmationRelevantFileNameSubstring = "Lomo.Commerce.MasterCard.Ftp.RebateConfirmationRelevantFileNameSubstring";
    }
}