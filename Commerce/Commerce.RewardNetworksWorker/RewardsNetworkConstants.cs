//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Commerce.RewardsNetworkWorker
{
    public static class RewardsNetworkConstants
    {
        /// <summary>
        /// Name of config entry that specifies whether reward networks jobs should be run or not.
        /// </summary>
        public static readonly string RunRewardsNetworkJobsPropertyName = "Lomo.Commerce.RunRewardsNetworkJobs";

        /// <summary>
        /// Name of config entry that specifies RewardsNetwork's FTP username.
        /// </summary>
        public static readonly string FtpUserName = "Lomo.Commerce.RewardsNetwork.Ftp.UserName";

        /// <summary>
        /// Name of config entry that specifies RewardsNetwork's FTP password
        /// </summary>
        public static readonly string FtpPassword = "Lomo.Commerce.RewardsNetwork.Ftp.Password";

        /// <summary>
        /// Name of config entry that specifies RewardsNetwork's FTP address
        /// </summary>
        public static readonly string FtpAddress = "Lomo.Commerce.RewardsNetwork.Ftp.Address";

        /// <summary>
        /// Name of config entry that specifies RewardsNetwork's FTP folder where the reports should be uploaded to.
        /// </summary>
        public static readonly string FtpTransactionReportDirectory = "Lomo.Commerce.RewardsNetwork.Ftp.TransactionReportDirectory";
    }
}