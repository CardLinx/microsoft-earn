//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.IO;
using OfferManagement.DataModel;
using System;

namespace OfferManagement.MerchantFileParser
{
    public abstract class MerchantFileProcessor
    {

        protected readonly IList<string> MCAuthFileImportColumnList = new List<string>
        {
            MerchantConstants.Name,
            MerchantConstants.Address,
            MerchantConstants.City,
            MerchantConstants.State,
            MerchantConstants.Zip,
            MerchantConstants.Phone,
            MerchantConstants.MCAcquiringICA,
            MerchantConstants.MCAcquiringMid,
            MerchantConstants.MCSiteId
        };

        protected readonly IList<string> MCClearingFileImportColumnList = new List<string>
        {
            MerchantConstants.Name,
            MerchantConstants.Address,
            MerchantConstants.City,
            MerchantConstants.State,
            MerchantConstants.Zip,
            MerchantConstants.Phone,
            MerchantConstants.MCLocationId,
            MerchantConstants.MCSiteId
        };

        protected readonly IList<string> ExportColumnList = new List<string>
        {
            MerchantConstants.Id,
            MerchantConstants.Name,
            MerchantConstants.Address,
            MerchantConstants.City,
            MerchantConstants.State,
            MerchantConstants.Zip,
            MerchantConstants.Phone
        };

        protected readonly IList<string> VisaImportList = new List<string>
        {
            MerchantConstants.Name,
            MerchantConstants.Address,
            MerchantConstants.City,
            MerchantConstants.State,
            MerchantConstants.Zip,
            MerchantConstants.VisaMid,
            MerchantConstants.VisaSid,
            MerchantConstants.PartnerMerchantId,
            MerchantConstants.VisaMidName,
            MerchantConstants.VisaSidName
        };

        protected readonly IList<string> AmexImportList = new List<string>
        {
            MerchantConstants.Id,
            MerchantConstants.Name,
            MerchantConstants.Address,
            MerchantConstants.City,
            MerchantConstants.State,
            MerchantConstants.Zip,
            MerchantConstants.Phone,
            MerchantConstants.AmexSENumber            
        };

        protected readonly IList<string> VisaExportColumnList = new List<string>
        {
           MerchantConstants.VisaMid,
           MerchantConstants.VisaSid
        };

        protected readonly IList<string> MCExportColumnList = new List<string>
        {
            MerchantConstants.MCAcquiringICA,
            MerchantConstants.MCAcquiringMid,
            MerchantConstants.MCLocationId
        };

        protected readonly IList<string> AmexExportColumnList = new List<string>
        {
           MerchantConstants.AmexSENumber
        };

        public abstract Tuple<string, IList<Merchant>> ImportMasterCardProvisioningFile(Stream fileStream);

        public abstract IList<Merchant> ImportMasterCardAuthFile(Stream fileStream);

        public abstract IList<Merchant> ImportMasterCardClearingFile(Stream fileStream);

        public abstract IList<Merchant> ImportVisaMidFile(Stream fileStream);

        public abstract IList<Merchant> ImportAmexMidFile(Stream fileStream);

        public abstract object GetMerchantsForExport(IList<Merchant> merchants, PaymentProcessor paymentProcessor, string providerId = null);

    }
}