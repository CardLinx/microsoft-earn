//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Security.Cryptography.X509Certificates;


namespace Visa.Proxy
{
    public class VisaRtmClientManager : IVisaRtmClientManager
    {
        public static readonly IVisaRtmClientManager Instance = new VisaRtmClientManager();

        private readonly VisaConfig config;
        private const string VisaQaCertThumbprint = "E9484CE9694742A782A6A1F7706E29A3E769E719";

        public VisaRtmClientManager()
        {
            config = new VisaConfig();
        }

        public RealTimeServiceClient GetVisaRtmClient()
        {
            var client = new RealTimeServiceClient("VisaRealTimeServiceEndPoint");

            if (client.ClientCredentials != null)
            {
                //TODO: Visa QA cert does not load since its chain is not correct. Unless it is fixed we set the last parameter of  Certificates.ByName to false so it can load
                var loadValidCertificateOnly = !string.Equals(VisaQaCertThumbprint, config.VisaClientCertificateThumbprint, StringComparison.OrdinalIgnoreCase);
                client.ClientCredentials.ClientCertificate.Certificate = Lomo.Core.Cryptography.Certificates.ByName(config.VisaClientCertificateThumbprint, StoreLocation.LocalMachine, X509FindType.FindByThumbprint, loadValidCertificateOnly);
            }

            var behavior = new PasswordDigestBehavior(config.VisaWsseUsername, config.VisaWssePassword);
            client.Endpoint.Behaviors.Add(behavior);
            return client;
        }
    }
}