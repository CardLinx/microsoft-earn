//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using OfferManagement.MerchantFileParser.MasterCard;
using System;
using System.IO;

namespace OfferManagement.MerchantFileParser
{
    public static class MerchantProcessorFactory
    {
        public static MerchantFileProcessor GetMerchantFileProcessor(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Missing filename");
            }
            if (!Path.HasExtension(fileName))
            {
                throw new ArgumentException(string.Format("Filename {0} is invalid", fileName));
            }

            string fileExtension = Path.GetExtension(fileName);
            MerchantFileProcessor merchantFileProcessor;
            switch (fileExtension.Trim().ToLower())
            {
                case ".xlsx":
                    merchantFileProcessor = new ExcelFileProcessor();
                    break;
                case ".csv":
                case ".txt":
                    merchantFileProcessor = new TextProcessor();
                    break;
                default:
                    throw new ArgumentException("Unsupported file format");
            }

            return merchantFileProcessor;
        }
    }
}