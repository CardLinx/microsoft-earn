//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Earn.Dashboard.Web.Attributes;
using Earn.Dashboard.Web.Models;
using Newtonsoft.Json;

namespace Earn.Dashboard.Web.Controllers
{
    [AuthorizeSG(Roles = "Admin,User")]
    public class DeveloperController : Controller
    {
        private const string CertName = "Earn.Dashboard.Web.offers-backend-services-management.pfx";

        private readonly XNamespace azureNs = "http://schemas.microsoft.com/windowsazure";

        private readonly List<ServiceDetails> serviceDetails = new List<ServiceDetails>
        {
            new ServiceDetails{
                Id = 1,
                Name = "User Services",
                SubscriptionId = "TODO_YOUR_SUBSCRIPTION_GUID_HERE",
                DnsName = "TODO_YOUR_TRAFFICMANAGER_DNSNAME_HERE",
                Profile = "PROFILE_NAME"
            },
            new ServiceDetails{
                Id = 2,
                Name = "Commerce",
                SubscriptionId = "TODO_YOUR_SUBSCRIPTION_GUID_HERE",
                DnsName = "TODO_YOUR_TRAFFICMANAGER_DNSNAME_HERE",
                Profile = "PROFILE_NAME"
            }
        };

        // GET: Developer
        public ActionResult Index()
        {
            return RedirectToAction("health");
        }

        public ActionResult Health()
        {
            return View(serviceDetails);
        }

        /// <summary>
        /// Gets the service details
        /// </summary>
        /// <param name="id">should be mapped to UI Id</param>
        /// <returns>The response</returns>
        public string GetServiceDetails(int id)
        {
            ServiceDetails detail = serviceDetails.Where(a => a.Id == id).FirstOrDefault();
            if (detail != null)
            {
                Certificate = GetCertificate();

                Uri uriWatm = new Uri(String.Format("https://management.core.windows.net/{0}/services/WATM/profiles/{1}/definitions", detail.SubscriptionId, detail.Profile));
                XDocument xdocumentWatm;
                HttpWebResponse responseWatm = InvokeRequest(uriWatm, "GET", out xdocumentWatm);

                if (responseWatm.StatusCode == HttpStatusCode.OK)
                {
                    ServiceInstance serviceInstance = new ServiceInstance
                    {
                        Name = detail.Profile,
                        Enabled = xdocumentWatm.Descendants(azureNs + "Status").FirstOrDefault().Value.Equals("Enabled"),
                        Status = "Online"
                    };

                    XElement relativePath = xdocumentWatm.Descendants(azureNs + "RelativePath").FirstOrDefault();
                    string path = relativePath != null ? relativePath.Value : string.Empty;

                    serviceInstance.Endpoint = "http://" + detail.DnsName + path;

                    IEnumerable<XElement> endpoints = xdocumentWatm.Descendants(azureNs + "Endpoint");
                    if (endpoints != null)
                    {
                        serviceInstance.Children = new List<ServiceInstance>();
                        foreach (var item in endpoints)
                        {
                            ServiceInstance endpoint = new ServiceInstance
                            {
                                Enabled = item.Element(azureNs + "Status") != null ? item.Element(azureNs + "Status").Value.Equals("Enabled") : false,
                                Endpoint = "http://" + item.Element(azureNs + "DomainName").Value + path,
                                Status = item.Element(azureNs + "MonitorStatus") != null ? item.Element(azureNs + "MonitorStatus").Value : null
                            };

                            string cloudservice = item.Element(azureNs + "DomainName").Value;
                            cloudservice = cloudservice.Remove(cloudservice.IndexOf(".cloudapp.net"));
                            endpoint.Name = cloudservice;

                            // Instances
                            Uri uriHostedservices = new Uri(String.Format("https://management.core.windows.net/{0}/services/hostedservices/{1}?embed-detail=true", detail.SubscriptionId, cloudservice));
                            XDocument xdocumentHostedservices;
                            HttpWebResponse responseHostedservices = InvokeRequest(uriHostedservices, "GET", out xdocumentHostedservices);

                            if (responseHostedservices.StatusCode == HttpStatusCode.OK)
                            {
                                IEnumerable<XElement> instances = xdocumentHostedservices
                                    .Descendants(azureNs + "Deployment")
                                    .Where(a => a.Element(azureNs + "DeploymentSlot").Value == "Production")
                                    .Descendants(azureNs + "RoleInstance");

                                if (instances != null)
                                {
                                    endpoint.Children = new List<ServiceInstance>();
                                    foreach (var inst in instances)
                                    {
                                        endpoint.Children.Add(new ServiceInstance
                                        {
                                            Name = inst.Element(azureNs + "InstanceName").Value,
                                            Enabled = true,
                                            Status = inst.Element(azureNs + "InstanceStatus") != null ? inst.Element(azureNs + "InstanceStatus").Value : null,
                                            Endpoint = "http://" + inst.Descendants(azureNs + "Vip").FirstOrDefault().Value + path
                                        });

                                    }
                                }
                            }

                            serviceInstance.Children.Add(endpoint);
                        }
                    }

                    return JsonConvert.SerializeObject(serviceInstance);
                }
            }

            return null;
        }

        private static X509Certificate2 Certificate { get; set; }

        private static HttpWebResponse InvokeRequest(Uri uri, string method, out XDocument responseBody)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = method;
            request.Headers.Add("x-ms-version", "2013-08-01");
            request.ClientCertificates.Add(Certificate);
            request.ContentType = "application/xml";

            responseBody = null;
            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;

            if (response.ContentLength > 0)
            {
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream(), settings))
                {
                    try
                    {
                        responseBody = XDocument.Load(reader);
                    }
                    catch
                    {
                        responseBody = null;
                    }
                }
            }
            response.Close();
            return response;
        }

        private static X509Certificate2 GetCertificate()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(CertName))
            {
                if (stream == null)
                {
                    throw new FileLoadException("An error occurred while loading the certificate", CertName);
                }

                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return new X509Certificate2(bytes, "TODO_YOUR_CERTIFICATE_PASSWORD_HERE", X509KeyStorageFlags.MachineKeySet
                    | X509KeyStorageFlags.PersistKeySet
                    | X509KeyStorageFlags.Exportable);
            }

            //List<StoreLocation> locations = new List<StoreLocation>
            //{ 
            //    StoreLocation.CurrentUser, 
            //    StoreLocation.LocalMachine
            //};

            //foreach (var location in locations)
            //{
            //    X509Store store = new X509Store("My", location);
            //    try
            //    {
            //        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            //        X509Certificate2Collection certificates = store.Certificates.Find(
            //        X509FindType.FindByThumbprint, Thumbprint, false);
            //        if (certificates.Count == 1)
            //        {
            //            return certificates[0];
            //        }
            //    }
            //    finally
            //    {
            //        store.Close();
            //    }
            //}

            //throw new ArgumentException(string.Format("A Certificate with Thumbprint '{0}' could not be located.", Thumbprint));
        }
    }
}