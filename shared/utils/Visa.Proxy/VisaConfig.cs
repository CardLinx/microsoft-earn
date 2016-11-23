//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Configuration;
using System.Linq;

namespace Visa.Proxy
{
    internal class VisaConfig
    {
        //public string VisaRrmServiceUrl { get; private set; }
        public string VisaClientCertificateThumbprint { get; private set; }
        public string VisaWsseUsername { get; private set; }
        public string VisaWssePassword { get; private set; }

        public VisaConfig()
        {
            //VisaRrmServiceUrl = ConfigurationManager.AppSettings["VisaRTMServiceUrl"];
            VisaWsseUsername = ConfigurationManager.AppSettings["VisaWsseUsername"];
            VisaWssePassword = ConfigurationManager.AppSettings["VisaWssePassword"];
            var thumbprint = ConfigurationManager.AppSettings["Visa.Client.Certificate.Thumbprint"];
            VisaClientCertificateThumbprint = new string(thumbprint.Where(c => c < 128).ToArray()); // remove the strange left-to-right mark is introduced 
        }
    }
}