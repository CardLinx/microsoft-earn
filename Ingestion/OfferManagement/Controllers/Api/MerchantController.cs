//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Azure.Utils;
using Lomo.Logging;
using Microsoft.HolMon.Security;
using Newtonsoft.Json;
using OfferManagement.Dal;
using OfferManagement.DataModel;
using OfferManagement.JobProcessor;
using OfferManagement.MerchantFileParser;
using OfferManagement.Models;

namespace OfferManagement.Controllers
{
    [ClaimsAuthorize(Roles = Roles.AllApisAccessRole)]
    [RoutePrefix("api/merchant")]
    public class MerchantController : ApiController
    {
        [Route("")]
        //[Route("Get/{id:Guid}")]
        [HttpGet]
        [ResponseType(typeof(Merchant))]
        public async Task<IHttpActionResult> Get(string id)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                Log.Warn("Invalid merchant id");
                return BadRequest("Invalid merchant id");
            }

            Log.Info("Serving MerchantController Get for id {0}", id);
            try
            {
                var merchant = (await EarnRepository.Instance.GetMerchantByIdAsync(id));
                if (merchant == null)
                {
                    Log.Warn("Merchant not found");
                    return NotFound();
                }

                return Ok(merchant);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing MerchantController Get for id {0}", id);
                return InternalServerError(ex);
            }
        }

        [Route("")]
        [HttpGet]
        [ResponseType(typeof(List<Merchant>))]
        public async Task<IHttpActionResult> GetByProviderId(string providerId, string query)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(providerId) || !Guid.TryParse(providerId, out guid))
            {
                Log.Warn("Invalid provider id");
                return BadRequest("Invalid provider id");
            }

            Log.Info("Serving MerchantController GetByProviderId for providerId {0}", providerId);
            try
            {
                var merchants = await EarnRepository.Instance.GetMerchantsForProviderAsync(providerId);
                if (merchants == null)
                {
                    Log.Warn("Merchant not found");
                    return NotFound();
                }

                return Ok(FilterMerchants(merchants, query));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing MerchantController GetByProviderId for providerId {0}", providerId);
                return InternalServerError(ex);
            }
        }

        [Route("import")]
        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> ImportMerchants([FromBody]ImportMerchantsModel merchantImportModel)
        {
            if (merchantImportModel == null)
            {
                return BadRequest();
            }

            Log.Info($"MerchantController ImportMerchants. Payload {merchantImportModel.ToString()}");
            Guid guid;

            string errorMessage = null;
            if (string.IsNullOrWhiteSpace(merchantImportModel.ProviderId) || !Guid.TryParse(merchantImportModel.ProviderId, out guid))
            {
                errorMessage = "Invalid ProviderId";
                Log.Error(errorMessage);
                return BadRequest(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(merchantImportModel.FileName))
            {
                errorMessage = "Missing merchant file name";
                Log.Error(errorMessage);
                return BadRequest(errorMessage);
            }

            if (!merchantImportModel.FileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase)
                && !merchantImportModel.FileName.EndsWith(".csv", StringComparison.InvariantCultureIgnoreCase))
            {
                errorMessage = "Invalid file extension for merchant file";
                Log.Error(errorMessage);
                return BadRequest(errorMessage);
            }

            try
            {
                var provider = (await EarnRepository.Instance.GetProviderAsync(merchantImportModel.ProviderId));
                if (provider == null)
                {
                    Log.Warn("Provider not found");
                    return NotFound();
                }

                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2 subscriptionCertificate = store.Certificates.Find(X509FindType.FindByThumbprint, ConfigurationManager.AppSettings["SchedulerCertificateThumbprint"], false)[0];
                store.Close();

                AzureScheduler azureScheduler = new AzureScheduler(subscriptionCertificate,
                    ConfigurationManager.AppSettings["SchedulerSubscriptionId"],
                    ConfigurationManager.AppSettings["SchedulerCloudServiceId"],
                    ConfigurationManager.AppSettings["SchedulerJobCollectionName"]);

                string jobId = Guid.NewGuid().ToString();
                ScheduledJobInfo scheduledJobInfo = new ScheduledJobInfo
                {
                    JobId = jobId,
                    JobScheduledTime = DateTime.UtcNow,
                    JobPayload = new Dictionary<string, string>
                        {
                            { JobConstants.ProviderId, merchantImportModel.ProviderId },
                            { JobConstants.ContainerName, ConfigurationManager.AppSettings["SchedulerStorageContainerName"] },
                            { JobConstants.BlobName, merchantImportModel.FileName },
                            { JobConstants.MerchantFileType, merchantImportModel.MerchantFileType.ToString() },
                            { JobConstants.Author, merchantImportModel.Author }
                        }
                };

                if (merchantImportModel.MerchantFileType == MerchantFileType.MasterCardAuth || merchantImportModel.MerchantFileType == MerchantFileType.MasterCardClearing)
                {
                    Log.Info($"Scheduling MasterCard Job for handling file type {merchantImportModel.MerchantFileType.ToString()}");
                    //Do not call visa MID lookup API if we are importing mastercard auth file. Merchant address from mastercard clearing file is always treated as ground truth
                    //Merchant address being the important parameter for MID lookup, it will be done at the time of clearing file import
                    string runVisaLookup = (merchantImportModel.MerchantFileType == MerchantFileType.MasterCardAuth) ? "false" : ConfigurationManager.AppSettings["RunVisaLookup"];

                    scheduledJobInfo.JobType = JobType.ProvisionMasterCardMid;
                    scheduledJobInfo.JobPayload.Add(JobConstants.RunVisaLookup, runVisaLookup);
                }
                else if (merchantImportModel.MerchantFileType == MerchantFileType.Visa)
                {
                    Log.Info($"Scheduling Visa Job for handling file type {merchantImportModel.MerchantFileType.ToString()}");
                    scheduledJobInfo.JobType = JobType.ProvisionRewardNetworkVisaMid;
                }
                else if (merchantImportModel.MerchantFileType == MerchantFileType.Amex)
                {
                    Log.Info($"Scheduling Amex Job for handling file type {merchantImportModel.MerchantFileType.ToString()}");
                    scheduledJobInfo.JobType = JobType.ProvisionAmexMid;
                }

                HttpStatusCode scheduleJobTask = await azureScheduler.ScheduleQueueTypeJobAsync(ConfigurationManager.AppSettings["SchedulerStorageAccountName"],
                    ConfigurationManager.AppSettings["SchedulerQueueName"],
                    ConfigurationManager.AppSettings["SchedulerSasToken"],
                    JsonConvert.SerializeObject(scheduledJobInfo),
                    jobId);

                if (scheduleJobTask == HttpStatusCode.OK || scheduleJobTask == HttpStatusCode.Created)
                {
                    Log.Info($"Successfully scheduled job");
                    return Ok(jobId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing MerchantController ImportMerchants");
                return InternalServerError(ex);
            }

            return InternalServerError();
        }

        [Route("export")]
        [HttpGet]
        public async Task<HttpResponseMessage> ExportMerchants(string providerId, PaymentProcessor paymentProcessor, string query)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(providerId) || !Guid.TryParse(providerId, out guid))
            {
                Log.Warn("Invalid provider id");
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Invalid provider id"
                };
            }

            Log.Info("Serving MerchantController ExportMerchants for providerId {0}", providerId);
            try
            {
                var merchants = await EarnRepository.Instance.GetMerchantsForProviderAsync(providerId);
                merchants = FilterMerchants(merchants, query);
                if (merchants == null)
                {
                    merchants = new List<Merchant>();
                }

                ExcelFileProcessor fileProcessor = new ExcelFileProcessor();
                byte[] result = (byte[])fileProcessor.GetMerchantsForExport(merchants.ToList(), paymentProcessor);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(result)
                };

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Merchants.xlsx"
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing MerchantController ExportMerchants for provider {0}", providerId);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = ex.Message
                };
            }
        }

        [Route("")]
        [HttpPost]
        [ResponseType(typeof(Merchant))]
        public async Task<IHttpActionResult> CreateOrUpdate([FromBody]Merchant merchant)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(merchant.ProviderId) || !Guid.TryParse(merchant.ProviderId, out guid))
            {
                Log.Error("Invalid provider id");
                return BadRequest("Invalid Provider Id");
            }

            Log.Info("Serving MerchantController CreateOrUpdate");
            try
            {
                Provider provider = (await EarnRepository.Instance.GetProviderAsync(merchant.ProviderId));
                if (provider == null)
                {
                    Log.Error("Provider not found");
                    return NotFound();
                }

                bool result;
                if (string.IsNullOrWhiteSpace(merchant.Id))
                {
                    // update provider with new count;
                    merchant.Id = Guid.NewGuid().ToString();
                    provider.TotalMerchants++;
                    result = await EarnRepository.Instance.CreateAsync<Merchant>(new List<Merchant> { merchant });
                    result &= await EarnRepository.Instance.UpdateAsync<Provider>(new List<Provider> { provider });

                    if (result)
                    {
                        return Created(result.ToString(), merchant);
                    }
                }
                else
                {
                    if (!Guid.TryParse(merchant.Id, out guid))
                    {
                        Log.Error("Invalid merchant id");
                        return BadRequest("Invalid merchant id");
                    }

                    Merchant existingMerchant = (await EarnRepository.Instance.GetMerchantByIdAsync(merchant.Id));
                    if (existingMerchant == null)
                    {
                        Log.Error($"Merchant record with Id {merchant.Id} not found");
                        return NotFound();
                    }

                    //Copy the non mutable properties to the new merchant document.
                    merchant.Id = existingMerchant.Id;
                    merchant.ExtendedAttributes = existingMerchant.ExtendedAttributes;
                    merchant.PartnerMerchantId = existingMerchant.PartnerMerchantId;
                    merchant.Images = existingMerchant.Images;
                    result = await EarnRepository.Instance.UpdateAsync<Merchant>(new List<Merchant> { merchant });
                    if (result)
                    {
                        return Ok(merchant);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing MerchantController CreateOrUpdate");
                return InternalServerError(ex);
            }

            return InternalServerError();
        }

        [Route("")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                Log.Warn("Invalid merchant id");
                return BadRequest("Invalid merchant id");
            }

            Log.Info("Serving MerchantController Delete for id {0}", id);
            try
            {
                var merchant = (await EarnRepository.Instance.GetMerchantByIdAsync(id));
                if (merchant == null)
                {
                    Log.Warn("Merchant not found");
                    return NotFound();
                }

                bool result = await EarnRepository.Instance.DeleteAsync(new List<string> { id });
                if (result)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing MerchantController Delete for id {0}", id);
                return InternalServerError(ex);
            }

            return InternalServerError();
        }

        private IEnumerable<Merchant> FilterMerchants(IEnumerable<Merchant> merchants, string query)
        {
            List<ExtendedMerchantModel> extendedMerchants = merchants.Select(x => new ExtendedMerchantModel(x)).ToList();

            if (merchants != null && !string.IsNullOrEmpty(query))
            {
                query = query
                    .Replace("Is = 'Valid'", "Valid")
                    .Replace("Is = 'Invalid'", "Invalid")
                    .Replace("Is = 'Synced'", "Synced")
                    .Replace("Is = 'PartiallySynced'", "PartiallySynced")
                    .Replace("Is = 'NotSynced'", "NotSynced")
                    .Replace("= 1", "= true")
                    .Replace("= 0", "= false")
                    .Replace("'", "\"");
                List<Merchant> result = new List<Merchant>();
                return extendedMerchants.Where(query).Select(x => x.Merchant).ToList();
            }
            return extendedMerchants.Select(x => x.Merchant);
        }
    }
}