//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    /// <summary>
    /// Constants related to FDC FTP Calls
    /// </summary>
    public static class FirstDataFtpConstants
    {
        /// <summary>
        /// Property Name for Flag to Run File Processing Job or not
        /// </summary>
        public static readonly string RunFirstDataFileProcessingJobsPropertyName = "Lomo.Commerce.RunFdcFileProcessingJobs";

        /// <summary>
        /// Property Name for FDC FTP Uri
        /// </summary>
        public static readonly string FirstDataFtpUriPropertyName = "Lomo.Commerce.Fdc.Ftp.Uri";

        /// <summary>
        /// FDC Extract FTP Username property
        /// </summary>
        public static readonly string FirstDataExtractFtpUserNamePropertyName = "Lomo.Commerce.Fdc.Ftp.Extract.Username";

        /// <summary>
        /// FDC Extract FTP Password property
        /// </summary>
        public static readonly string FirstDataExtractFtpPasswordPropertyName = "Lomo.Commerce.Fdc.Ftp.Extract.Password";

        /// <summary>
        /// FDC Extract FTP Directory property
        /// </summary>
        public static readonly string FirstDataExtractDirectoryPropertyName = "Lomo.Commerce.Fdc.Ftp.Extract.RemoteDirectory";

        /// <summary>
        /// FDC Pts FTP Username property
        /// </summary>
        public static readonly string FirstDataPtsFtpUserNamePropertyName = "Lomo.Commerce.Fdc.Ftp.Pts.Username";

        /// <summary>
        /// FDC Pts FTP Password property
        /// </summary>
        public static readonly string FirstDataPtsFtpPasswordPropertyName = "Lomo.Commerce.Fdc.Ftp.Pts.Password";

        /// <summary>
        /// FDC Pts FTP Directory property
        /// </summary>
        public static readonly string FirstDataPtsDirectoryPropertyName = "Lomo.Commerce.Fdc.Ftp.Pts.RemoteDirectory";

        /// <summary>
        /// FDC Ack FTP Username property
        /// </summary>
        public static readonly string FirstDataAcknowledgmentFtpUserNamePropertyName = "Lomo.Commerce.Fdc.Ftp.Ack.Username";

        /// <summary>
        /// FDC Ack FTP Password property
        /// </summary>
        public static readonly string FirstDataAcknowledgmentFtpPasswordPropertyName = "Lomo.Commerce.Fdc.Ftp.Ack.Password";

        /// <summary>
        /// FDC Ack FTP Directory property
        /// </summary>
        public static readonly string FirstDataAcknowledgmentDirectoryPropertyName = "Lomo.Commerce.Fdc.Ftp.Ack.RemoteDirectory";

    }
}