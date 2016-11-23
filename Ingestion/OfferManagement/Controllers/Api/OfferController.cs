//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
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

namespace OfferManagement.Controllers
{
    [ClaimsAuthorize(Roles = Roles.AllApisAccessRole)]
    [RoutePrefix("api/offer")]
    public class OfferController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(Offer))]
        public async Task<IHttpActionResult> Get(string id)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                Log.Warn("Invalid offer id");
                return BadRequest("Invalid offer id");
            }

            Log.Info("Serving OfferController Get for id {0}", id);
            try
            {
                var offer = (await EarnRepository.Instance.GetOfferAsync(id));
                if (offer == null)
                {
                    Log.Warn("Offer not found");
                    return NotFound();
                }

                return Ok(offer);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing OfferController Get for id {0}", id);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ResponseType(typeof(List<Offer>))]
        public async Task<IHttpActionResult> GetByProviderId(string providerId)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(providerId) || !Guid.TryParse(providerId, out guid))
            {
                Log.Warn("Invalid provider id");
                return BadRequest("Invalid provider id");
            }

            Log.Info("Serving OfferController GetByProviderId for providerId {0}", providerId);
            try
            {
                var offers = await EarnRepository.Instance.GetOffersForProviderAsync(providerId);
                if (offers == null)
                {
                    Log.Warn("Offer not found");
                    return NotFound();
                }

                return Ok(offers);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing OfferController GetByProviderId for providerId {0}", providerId);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ResponseType(typeof(Offer))]
        public async Task<IHttpActionResult> CreateOrUpdate([FromBody]Offer offer, bool active = false)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(offer.ProviderId) || !Guid.TryParse(offer.ProviderId, out guid))
            {
                Log.Warn("Invalid provider id");
                return BadRequest("Invalid provider id");
            }

            Log.Info("Serving OfferController CreateOrUpdate");
            try
            {
                Provider provider = (await EarnRepository.Instance.GetProviderAsync(offer.ProviderId));
                if (provider == null)
                {
                    Log.Warn("Provider not found");
                    return NotFound();
                }

                bool result;
                bool isNew;

                if (active)
                {
                    offer.StartDate = DateTime.UtcNow;
                    offer.EndDate = DateTime.MaxValue;
                }
                else
                {
                    offer.StartDate = offer.EndDate = DateTime.MinValue;
                }

                if (string.IsNullOrWhiteSpace(offer.Id))
                {
                    isNew = true;
                    offer.Id = Guid.NewGuid().ToString();
                    result = await EarnRepository.Instance.CreateAsync(new List<Offer> { offer });
                }
                else
                {
                    isNew = false;
                    if (!Guid.TryParse(offer.Id, out guid))
                    {
                        Log.Warn("Invalid offer id");
                        return BadRequest("Invalid Offer Id");
                    }

                    // check if this offer exists
                    Offer previousOffer = (await EarnRepository.Instance.GetOfferAsync(offer.Id));
                    if (previousOffer == null)
                    {
                        Log.Warn("Offer not found");
                        return NotFound();
                    }

                    result = await EarnRepository.Instance.UpdateAsync(new List<Offer> { offer });
                }

                if (active)
                {
                    //If the offer is active, update the provider with the active offer
                    Log.Verbose("Setting offer {0} as active for the provider {1}", offer.Id, offer.ProviderId);
                    provider.OfferId = offer.Id;
                    result &= await EarnRepository.Instance.UpdateAsync(new List<Provider> { provider });

                    Log.Verbose("Scheduling the job to register the offer with commerce");
                    //Schedule the job to register the offer, provider and merchant mids with commerce
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
                        JobType = JobType.SyncOfferWithCommerce,
                        JobScheduledTime = DateTime.UtcNow,
                        JobPayload = new Dictionary<string, string>
                        {
                            { JobConstants.OfferId, offer.Id }
                        }
                    };

                    HttpStatusCode scheduleJobTask = await azureScheduler.ScheduleQueueTypeJobAsync(ConfigurationManager.AppSettings["SchedulerStorageAccountName"],
                        ConfigurationManager.AppSettings["SchedulerQueueName"],
                        ConfigurationManager.AppSettings["SchedulerSasToken"],
                        JsonConvert.SerializeObject(scheduledJobInfo),
                        jobId);

                    if (scheduleJobTask == HttpStatusCode.OK || scheduleJobTask == HttpStatusCode.Created)
                    {
                        Log.Verbose("Successfully scheduled the job to register the offer with commerce");
                    }
                }

                if (result)
                {
                    if (isNew)
                    {
                        Log.Verbose("Offer created - id {0}", offer.Id);
                        return Created(result.ToString(), offer);
                    }

                    Log.Verbose("Offer updated - id {0}", offer.Id);

                    return Ok(offer);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing OfferController CreateOrUpdate");
                return InternalServerError(ex);
            }

            return InternalServerError();
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                Log.Warn("Invalid offer id");
                return BadRequest("Invalid offer id");
            }

            Log.Info("Serving OfferController Delete for id {0}", id);
            try
            {
                var offer = await EarnRepository.Instance.GetOfferAsync(id).ConfigureAwait(false);
                if (offer == null)
                {
                    Log.Warn("Offer not found");
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
                Log.Error(ex, "Failed processing OfferController Delete for id {0}", id);
                return InternalServerError(ex);
            }

            return InternalServerError();
        }
    }
}