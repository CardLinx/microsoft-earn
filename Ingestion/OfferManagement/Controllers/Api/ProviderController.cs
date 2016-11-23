//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using OfferManagement.Dal;
using OfferManagement.DataModel;
using Microsoft.HolMon.Security;
using Lomo.Logging;

namespace OfferManagement.Controllers
{
    [ClaimsAuthorize(Roles = Roles.AllApisAccessRole)]
    [RoutePrefix("api/provider")]
    public class ProviderController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(List<Provider>))]
        public async Task<IHttpActionResult> Get()
        {
            Log.Info("Serving ProviderController Get all");
            try
            {
                return Ok(await EarnRepository.Instance.GetAllProvidersAsync());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing ProviderController Get");
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ResponseType(typeof(Provider))]
        public async Task<IHttpActionResult> Get(string id)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                Log.Warn("Invalid provider id");
                return BadRequest("Invalid provider id");
            }

            Log.Info("Serving ProviderController Get for id {0}", id);
            try
            {
                var provider = (await EarnRepository.Instance.GetProviderAsync(id));
                if (provider == null)
                {
                    Log.Warn("Provider not found");
                    return NotFound();
                }

                return Ok(provider);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing ProviderController Get for id {0}", id);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ResponseType(typeof(Provider))]
        public async Task<IHttpActionResult> CreateOrUpdate([FromBody]Provider provider)
        {
            if (provider == null || string.IsNullOrWhiteSpace(provider.Name))
            {
                Log.Warn("Invalid provider name");
                return BadRequest("Invalid provider name");
            }

            Log.Info("Serving ProviderController CreateOrUpdate");
            try
            {
                bool result;
                if (string.IsNullOrWhiteSpace(provider.Id))
                {
                    provider.Id = Guid.NewGuid().ToString();
                    result = await EarnRepository.Instance.CreateAsync<Provider>(new List<Provider> { provider });
                    if (result)
                    {
                        Log.Verbose("Created provider - id {0}", provider.Id);
                        return Created(result.ToString(), provider);
                    }
                }
                else
                {
                    Guid guid;
                    if (!Guid.TryParse(provider.Id, out guid))
                    {
                        Log.Warn("Invalid provider id");
                        return BadRequest("Invalid Provider id");
                    }

                    Provider previousProvider = (await EarnRepository.Instance.GetProviderAsync(provider.Id));
                    if (previousProvider == null)
                    {
                        Log.Warn("Provider not found");
                        return NotFound();
                    }

                    Log.Verbose("Updated provider - id {0}", provider.Id);
                    result = await EarnRepository.Instance.UpdateAsync<Provider>(new List<Provider> { provider });
                    if (result)
                    {
                        return Ok(provider);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing ProviderController CreateOrUpdate");
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
                Log.Warn("Invalid provider id");
                return BadRequest("Invalid provider id");
            }

            Log.Info("Serving ProviderController Delete for id {0}", id);
            try
            {
                var provider = (await EarnRepository.Instance.GetProviderAsync(id));
                if (provider == null)
                {
                    Log.Warn("Provider not found");
                    return NotFound();
                }

                List<string> deleteList = new List<string>() { id };
                var merchants = await EarnRepository.Instance.GetMerchantsForProviderAsync(id);
                if (merchants != null && merchants.Any())
                    deleteList.AddRange(merchants.Select(m => m.Id));

                var offers = await EarnRepository.Instance.GetOffersForProviderAsync(id);
                if (offers != null && offers.Any())
                    deleteList.AddRange(offers.Select(o => o.Id));

                bool result = await EarnRepository.Instance.DeleteAsync(deleteList);
                if (result)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed processing ProviderController Delete for id {0}", id);
                return InternalServerError(ex);
            }

            return InternalServerError();
        }
    }
}