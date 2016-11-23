//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Lomo.Commerce.DataContracts;
using Newtonsoft.Json;
using System;
using Lomo.Logging;

namespace OfferManagement.JobProcessor
{

    public class CommerceService : ICommerceService
    {
        string commerceUrlEndPoint;

        string certThumbPrint;

        public CommerceService(string commerceUrlEndPoint, string certThumbPrint)
        {
            this.commerceUrlEndPoint = commerceUrlEndPoint;
            this.certThumbPrint = certThumbPrint;
        }

        public CommerceResponse RegisterOffer(string offerPayload)
        {            
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(commerceUrlEndPoint);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            X509Certificate2 cert = Lomo.Core.Cryptography.Certificates.ByName(certThumbPrint, StoreLocation.LocalMachine,
                           X509FindType.FindByThumbprint, false);

            httpWebRequest.ClientCertificates.Add(cert);
            byte[] requestBodyBytes = Encoding.UTF8.GetBytes(offerPayload);
            httpWebRequest.ContentLength = requestBodyBytes.Length;
            using (Stream stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
            }

            var response = (HttpWebResponse)httpWebRequest.GetResponse();
            return ExtractCommerceResponse(response);            
        }

        private CommerceResponse ExtractCommerceResponse(HttpWebResponse response)
        {
            CommerceResponse result = null;
            try
            {
                Stream responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    string responseText;
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        responseText = streamReader.ReadToEnd();
                    }
                    if (!string.IsNullOrWhiteSpace(responseText))
                    {
                        result = JsonConvert.DeserializeObject<V3RegisterDealResponse>(responseText);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

            return result;
        }


    }
}
