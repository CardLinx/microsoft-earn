//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Excel;
using OfferManagement.DataModel;
using OfficeOpenXml;
using Utilities;
using Log = Lomo.Logging.Log;

namespace OfferManagement.MerchantFileParser
{
    public class ExcelFileProcessor : MerchantFileProcessor
    {
        public override IList<Merchant> ImportMasterCardAuthFile(Stream fileStream)
        {
            IList<Merchant> lstMerchants = new List<Merchant>();
            try
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                excelReader.IsFirstRowAsColumnNames = true;

                DataSet storeDataSet = excelReader.AsDataSet();
                DataTable storeDataTable = storeDataSet.Tables[0];
                if (storeDataTable != null)
                {
                    Log.Info("Total Rows in MasterCardAuthFile is {0}", storeDataTable.Rows.Count);
                    var lstMerchantData = GetDataFromSpecifiedColumns(storeDataTable, MCAuthFileImportColumnList);
                    Log.Info("Total Rows read from MasterCardAuthFile is {0}", lstMerchantData.Count);

                    foreach (var merchantData in lstMerchantData)
                    {
                        Merchant existingMerchant = null;

                        //Check if we have already seen this merchant in this spreadsheet based on the siteid of the merchant
                        string siteId = merchantData.ContainsKey(MerchantConstants.MCSiteId) ? merchantData[MerchantConstants.MCSiteId] : string.Empty;
                        if (!string.IsNullOrEmpty(siteId))
                        {
                            existingMerchant = lstMerchants
                            .FirstOrDefault(m => (m.ExtendedAttributes != null && m.ExtendedAttributes.ContainsKey(MerchantConstants.MCSiteId)
                            && m.ExtendedAttributes[MerchantConstants.MCSiteId] == merchantData[MerchantConstants.MCSiteId]));
                        }

                        //if we have not seen the merchant, then create one
                        if (existingMerchant == null)
                        {
                            var merchant = CreateMerchant(merchantData);
                            merchant.Payments = new List<Payment>();
                            Payment payment = new Payment();
                            payment.Processor = PaymentProcessor.MasterCard;
                            payment.PaymentMids = new Dictionary<string, string>();
                            payment.PaymentMids.Add(MerchantConstants.MCAcquiringICA, merchantData[MerchantConstants.MCAcquiringICA]);
                            payment.PaymentMids.Add(MerchantConstants.MCAcquiringMid, merchantData[MerchantConstants.MCAcquiringMid]);
                            merchant.Payments.Add(payment);
                            merchant.ExtendedAttributes = new Dictionary<string, string>();
                            merchant.ExtendedAttributes.Add(MerchantConstants.MCSiteId, siteId);
                            lstMerchants.Add(merchant);
                        }
                        else
                        {
                            //else, update the payment info to the existing merchant
                            Payment payment = new Payment();
                            payment.Processor = PaymentProcessor.MasterCard;
                            payment.PaymentMids = new Dictionary<string, string>();
                            payment.PaymentMids.Add(MerchantConstants.MCAcquiringICA, merchantData[MerchantConstants.MCAcquiringICA]);
                            payment.PaymentMids.Add(MerchantConstants.MCAcquiringMid, merchantData[MerchantConstants.MCAcquiringMid]);
                            existingMerchant.Payments.Add(payment);
                        }
                    }
                }

                Log.Info("Total unique merchants read from MasterCardAuth file is {0} ", lstMerchants.Count);

                return lstMerchants;
            }
            catch (Exception e)
            {
                Log.Error("Error in reading merchant data from excel stream " + e.Message);
                throw;
            }
        }

        public override IList<Merchant> ImportMasterCardClearingFile(Stream fileStream)
        {
            IList<Merchant> lstMerchants = new List<Merchant>();
            try
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                excelReader.IsFirstRowAsColumnNames = true;

                DataSet storeDataSet = excelReader.AsDataSet();
                DataTable storeDataTable = storeDataSet.Tables[0];
                if (storeDataTable != null)
                {
                    Log.Info("Total Rows in MasterCardClearingFile is {0}", storeDataTable.Rows.Count);
                    var lstMerchantData = GetDataFromSpecifiedColumns(storeDataTable, MCClearingFileImportColumnList);
                    Log.Info("Total Rows read from MasterCardClearingFile is {0}", lstMerchantData.Count);

                    foreach (var merchantData in lstMerchantData)
                    {
                        Merchant existingMerchant = null;

                        //Check if we have already seen this merchant in this spreadsheet based on the siteid of the merchant
                        string siteId = merchantData.ContainsKey(MerchantConstants.MCSiteId) ? merchantData[MerchantConstants.MCSiteId] : string.Empty;
                        if (!string.IsNullOrEmpty(siteId))
                        {
                            existingMerchant = lstMerchants
                            .FirstOrDefault(m => (m.ExtendedAttributes != null && m.ExtendedAttributes.ContainsKey(MerchantConstants.MCSiteId)
                            && m.ExtendedAttributes[MerchantConstants.MCSiteId] == merchantData[MerchantConstants.MCSiteId]));
                        }

                        //if we have not seen the merchant, then create one
                        if (existingMerchant == null)
                        {
                            var merchant = CreateMerchant(merchantData);
                            merchant.ExtendedAttributes = new Dictionary<string, string>();
                            merchant.ExtendedAttributes.Add(MerchantConstants.MCSiteId, merchantData[MerchantConstants.MCSiteId]);
                            merchant.Payments = new List<Payment>();
                            Payment payment = new Payment();
                            payment.Processor = PaymentProcessor.MasterCard;
                            payment.PaymentMids = new Dictionary<string, string>();
                            payment.PaymentMids.Add(MerchantConstants.MCLocationId, merchantData[MerchantConstants.MCLocationId]);
                            merchant.Payments.Add(payment);
                            lstMerchants.Add(merchant);
                        }
                        else
                        {
                            //else, update the payment info to the existing merchant
                            Payment payment = new Payment();
                            payment.Processor = PaymentProcessor.MasterCard;
                            payment.PaymentMids = new Dictionary<string, string>();
                            payment.PaymentMids.Add(MerchantConstants.MCLocationId, merchantData[MerchantConstants.MCLocationId]);
                            existingMerchant.Payments.Add(payment);
                        }
                    }
                }

                Log.Info("Total unique merchants read from MasterCardClearing file is {0} ", lstMerchants.Count);

                return lstMerchants;
            }
            catch (Exception e)
            {
                Log.Error("Error in reading merchant data from excel stream " + e.Message);
                throw;
            }
        }

        public override IList<Merchant> ImportVisaMidFile(Stream fileStream)
        {
            IList<Merchant> lstMerchants = new List<Merchant>();
            try
            {
                HashSet<string> visaMids = new HashSet<string>();
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                excelReader.IsFirstRowAsColumnNames = true;

                DataSet storeDataSet = excelReader.AsDataSet();
                DataTable storeDataTable = storeDataSet.Tables[0];
                if (storeDataTable != null)
                {
                    Log.Info("Total Rows in VisaMid File is {0}", storeDataTable.Rows.Count);
                    var lstMerchantData = GetDataFromSpecifiedColumns(storeDataTable, VisaImportList);
                    Log.Info("Total Rows read from VisaMid File is {0}", lstMerchantData.Count);

                    foreach (var merchantData in lstMerchantData)
                    {
                        Merchant existingMerchant = null;

                        //Check if we have already seen this merchant in this spreadsheet based on the PartnerMerchantId of the merchant
                        string partnerMerchantId = merchantData.ContainsKey(MerchantConstants.PartnerMerchantId) ? merchantData[MerchantConstants.PartnerMerchantId] : null;
                        if (!string.IsNullOrEmpty(partnerMerchantId))
                        {
                            existingMerchant = lstMerchants.FirstOrDefault(m => m.PartnerMerchantId == merchantData[MerchantConstants.PartnerMerchantId]);
                        }
                        if (existingMerchant == null)
                        {
                            Merchant merchant = CreateMerchant(merchantData);
                            merchant.PartnerMerchantId = partnerMerchantId;
                            Payment payment = CreateVisaPayment(merchantData, visaMids);
                            merchant.Payments = new List<Payment>();
                            merchant.Payments.Add(payment);

                            merchant.ExtendedAttributes = new Dictionary<string, string>();
                            string visaMidName = merchantData.ContainsKey(MerchantConstants.VisaMidName) ? merchantData[MerchantConstants.VisaMidName] : null;
                            string visaSidName = merchantData.ContainsKey(MerchantConstants.VisaSidName) ? merchantData[MerchantConstants.VisaSidName] : null;
                            if (!string.IsNullOrEmpty(visaMidName))
                            {
                                merchant.ExtendedAttributes.Add(MerchantConstants.VisaMidName, visaMidName);
                            }
                            if (!string.IsNullOrEmpty(visaSidName))
                            {
                                merchant.ExtendedAttributes.Add(MerchantConstants.VisaSidName, visaSidName);
                            }
                            lstMerchants.Add(merchant);
                        }
                        else
                        {
                            Payment payment = CreateVisaPayment(merchantData, visaMids);
                            existingMerchant.Payments.Add(payment);
                        }
                    }
                }

                Log.Info("Total unique merchants read from VisaMid file is {0} ", lstMerchants.Count);

                return lstMerchants;
            }
            catch (Exception e)
            {
                Log.Error("Error in reading merchant data from excel stream " + e.Message);
                throw;
            }
        }

        public override IList<Merchant> ImportAmexMidFile(Stream fileStream)
        {
            IList<Merchant> merchants = new List<Merchant>();
            try
            {
                HashSet<string> amexMids = new HashSet<string>();
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                excelReader.IsFirstRowAsColumnNames = true;

                DataSet storeDataSet = excelReader.AsDataSet();
                DataTable storeDataTable = storeDataSet.Tables[0];
                if (storeDataTable != null)
                {
                    Log.Info("Total Rows in AmexMid File is {0}", storeDataTable.Rows.Count);
                    var merchantData = GetDataFromSpecifiedColumns(storeDataTable, AmexImportList);
                    Log.Info("Total Rows read from AmexMid File is {0}", merchantData.Count);

                    foreach (var data in merchantData)
                    {
                        Merchant existingMerchant = null;
                        //Check if we have already seen this merchant in this spreadsheet based on the PartnerMerchantId of the merchant
                        string merchantId = data.ContainsKey(MerchantConstants.Id) ? data[MerchantConstants.Id] : null;
                        if (!string.IsNullOrEmpty(merchantId))
                        {
                            existingMerchant = merchants.FirstOrDefault(m => m.Id == data[MerchantConstants.Id]);
                        }

                        if (existingMerchant == null)
                        {
                            Merchant merchant = new Merchant
                            {
                                Id = data[MerchantConstants.Id],
                                Name = data[MerchantConstants.Name],
                                Location = new Location
                                {
                                    Address = data[MerchantConstants.Address],
                                    City = data[MerchantConstants.City],
                                    State = data[MerchantConstants.State],
                                    Zip = data[MerchantConstants.Zip]
                                },
                                PhoneNumber = data.ContainsKey(MerchantConstants.Phone)
                                    ? StringUtility.StripAllButDigits(data[MerchantConstants.Phone]) : string.Empty
                            };

                            // amexMids hashset is passed to validate duplication of SE numbers for different merchants                  
                            merchant.Payments = CreateAmexPayment(data, amexMids);
                            merchants.Add(merchant);
                        }
                        else
                        {
                            // if MerchantId appears multiple times in the list hence add to the list
                            var payments = CreateAmexPayment(data, amexMids);
                            if (payments != null && payments.Any())
                            {
                                foreach (var payment in payments)
                                {
                                    existingMerchant.Payments.Add(payment);
                                }
                            }
                        }
                    }
                }

                Log.Info("Total unique merchants read from AmexMid file is {0} ", merchants.Count);
                return merchants;
            }
            catch (Exception e)
            {
                Log.Error("Error in reading merchant data from excel stream " + e.Message);
                throw;
            }
        }

        private Payment CreateVisaPayment(Dictionary<string, string> merchantData, HashSet<string> visaMids)
        {
            Payment payment = new Payment();
            payment.Processor = PaymentProcessor.Visa;
            payment.PaymentMids = new Dictionary<string, string>();
            string vmid = merchantData[MerchantConstants.VisaMid];
            string vsid = merchantData[MerchantConstants.VisaSid];
            string visaMid = $"{vmid};{vsid}";
            if (!string.IsNullOrWhiteSpace(vmid) && !string.IsNullOrWhiteSpace(vsid) && !visaMids.Contains(visaMid))
            {
                payment.PaymentMids.Add(MerchantConstants.VisaMid, merchantData[MerchantConstants.VisaMid]);
                payment.PaymentMids.Add(MerchantConstants.VisaSid, merchantData[MerchantConstants.VisaSid]);
                visaMids.Add(visaMid);
            }

            return payment;
        }

        private List<Payment> CreateAmexPayment(Dictionary<string, string> merchantData, HashSet<string> amexMids)
        {
            string merchantSeNumbers = merchantData[MerchantConstants.AmexSENumber];
            List<Payment> payments = new List<Payment>();
            if (!string.IsNullOrWhiteSpace(merchantSeNumbers))
            {
                var seNumbers = merchantSeNumbers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var seNumber in seNumbers)
                {
                    if (!string.IsNullOrWhiteSpace(seNumber) && !amexMids.Contains(seNumber))
                    {
                        payments.Add(new Payment
                        {
                            Processor = PaymentProcessor.Amex,
                            PaymentMids = new Dictionary<string, string>() { { MerchantConstants.AmexSENumber, seNumber.Trim() } }
                        });

                        amexMids.Add(seNumber);
                    }
                }
            }

            return payments;
        }

        private Merchant CreateMerchant(Dictionary<string, string> merchantData)
        {
            Merchant merchant = new Merchant
            {
                Id = Guid.NewGuid().ToString(),
                Name = merchantData[MerchantConstants.Name],
                Location = new Location
                {
                    Address = merchantData[MerchantConstants.Address],
                    City = merchantData[MerchantConstants.City],
                    State = merchantData[MerchantConstants.State],
                    Zip = merchantData[MerchantConstants.Zip]
                },
                PhoneNumber = merchantData.ContainsKey(MerchantConstants.Phone)
                    ? StringUtility.StripAllButDigits(merchantData[MerchantConstants.Phone])
                    : string.Empty
            };

            return merchant;
        }

        public override object GetMerchantsForExport(IList<Merchant> merchants, PaymentProcessor paymentProcessor, string providerId = null)
        {
            ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Merchant Data");
            AddColumns(paymentProcessor, worksheet);
            AddRows(merchants, worksheet);

            return excelPackage.GetAsByteArray();
        }

        private void AddColumns(PaymentProcessor paymentProcessor, ExcelWorksheet worksheet)
        {
            int columnNumber = 1;
            AddCommonColumnHeaders(worksheet, ref columnNumber);
            switch (paymentProcessor)
            {
                case PaymentProcessor.Visa:
                    AddVisaColumnHeaders(worksheet, ref columnNumber);
                    break;
                case PaymentProcessor.MasterCard:
                    AddMasterCardColumnHeaders(worksheet, ref columnNumber);
                    break;
                case PaymentProcessor.Amex:
                    AddAmexColumnHeaders(worksheet, ref columnNumber);
                    break;
            }
        }

        private static void AddRows(IList<Merchant> merchants, ExcelWorksheet worksheet)
        {
            int rowNumber = 2;
            foreach (var merchant in merchants)
            {
                int columnNumber = 1;
                worksheet.Cells[rowNumber, columnNumber].Value = merchant.Id;
                columnNumber++;

                worksheet.Cells[rowNumber, columnNumber].Value = merchant.Name;
                columnNumber++;

                worksheet.Cells[rowNumber, columnNumber].Value = merchant.Location.Address;
                columnNumber++;

                worksheet.Cells[rowNumber, columnNumber].Value = merchant.Location.City;
                columnNumber++;

                worksheet.Cells[rowNumber, columnNumber].Value = merchant.Location.State;
                columnNumber++;

                worksheet.Cells[rowNumber, columnNumber].Value = merchant.Location.Zip;
                columnNumber++;

                worksheet.Cells[rowNumber, columnNumber].Value = merchant.PhoneNumber;

                rowNumber++;
            }
        }

        private void AddCommonColumnHeaders(ExcelWorksheet worksheet, ref int columnNumber)
        {
            foreach (string columnName in ExportColumnList)
            {
                worksheet.Cells[1, columnNumber].Value = columnName;
                columnNumber++;
            }
        }

        private void AddVisaColumnHeaders(ExcelWorksheet worksheet, ref int columnNumber)
        {
            foreach (string columnName in VisaExportColumnList)
            {
                worksheet.Cells[1, columnNumber].Value = columnName;
                columnNumber++;
            }
        }

        private void AddMasterCardColumnHeaders(ExcelWorksheet worksheet, ref int columnNumber)
        {
            foreach (string columnName in MCExportColumnList)
            {
                worksheet.Cells[1, columnNumber].Value = columnName;
                columnNumber++;
            }
        }

        private void AddAmexColumnHeaders(ExcelWorksheet worksheet, ref int columnNumber)
        {
            foreach (string columnName in AmexExportColumnList)
            {
                worksheet.Cells[1, columnNumber].Value = columnName;
                columnNumber++;
            }
        }

        private List<Dictionary<string, string>> GetDataFromSpecifiedColumns(DataTable storeDataTable, IList<string> dataColumns)
        {
            List<Dictionary<string, string>> lstMerchantData = new List<Dictionary<string, string>>();
            for (int row = 0; row < storeDataTable.Rows.Count; row++)
            {
                Dictionary<string, string> merchantData = new Dictionary<string, string>();
                foreach (string column in dataColumns)
                {
                    DataColumn dataColumn = storeDataTable.Columns[column];
                    if (dataColumn != null)
                    {
                        string value = storeDataTable.Rows[row].ItemArray[dataColumn.Ordinal].ToString().Trim();
                        merchantData[column] = value;
                    }
                }

                lstMerchantData.Add(merchantData);
            }

            return lstMerchantData;
        }

        public override Tuple<string, IList<Merchant>> ImportMasterCardProvisioningFile(Stream fileStream)
        {
            throw new NotImplementedException();
        }

    }
}