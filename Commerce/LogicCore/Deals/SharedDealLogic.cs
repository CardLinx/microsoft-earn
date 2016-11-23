//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Service;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains helper methods to perform shared business logic for Deal objects.
    /// </summary>
    public class SharedDealLogic
    {
        /// <summary>
        /// Initializes a new instance of the SharedDealLogic class.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        /// <param name="dealOperations">
        /// The object to use to perform operations on deals.
        /// </param>
        public SharedDealLogic(CommerceContext context,
                               IDealOperations dealOperations)
        {
            Context = context;
            DealOperations = dealOperations;
            ProviderOperations = new ProviderOperations();
            ProviderOperations.Context = context;
            MerchantOperations = new MerchantOperations();
            MerchantOperations.Context = context;
            OfferOperations = new OfferOperations();
            OfferOperations.Context = context;
        }

        /// <summary>
        /// Executes the Register offer API invocation.
        public void Execute()
        {
            ResultCode resultCode;

            V3RegisterDealResponse response = (V3RegisterDealResponse)Context[Key.Response]; // Populated in controller.
            response.DiscountResults = new Dictionary<Guid, string>();

            V3DealDataContract v3DealDataContract = (V3DealDataContract)Context[Key.DealDataContract]; // Populated in controller.

            // Build the provider object.
            Provider provider = null;
            resultCode = BuildProvider(v3DealDataContract, out provider);

            // If we're processing a national provider, it must contain exactly one merchant.
            if (resultCode == ResultCode.Success && v3DealDataContract.IsNational == true && v3DealDataContract.Discounts.Count() != 1)
            {
                resultCode = ResultCode.InvalidDeal;
            }

            // Build the merchant objects.
            List<Merchant> merchants = new List<Merchant>();
            if (resultCode == ResultCode.Success)
            {
                foreach (V3DiscountDataContract v3DiscountDataContract in v3DealDataContract.Discounts)
                {
                    resultCode = BuildMerchant(v3DiscountDataContract, v3DealDataContract.ProviderId, v3DealDataContract.IsNational, merchants);
                    if (resultCode != ResultCode.Success)
                    {
                        break;
                    }
                }
            }

            // Build the offer object.
            Offer offer = null;
            if (resultCode == ResultCode.Success)
            {
                resultCode = BuildOffer(v3DealDataContract, out offer);
            }
                        
            // Commit the provider to the data store.
            if (resultCode == ResultCode.Success)
            {
                resultCode = AddOrUpdateProvider(provider);
            }

            // Commit the merchants to the data store.
            if (resultCode == ResultCode.Success || resultCode == ResultCode.Created)
            {
                foreach (Merchant merchant in merchants)
                {
                    resultCode = AddOrUpdateMerchant(merchant);
                    if (resultCode != ResultCode.Success && resultCode != ResultCode.Created)
                    {
                        break;
                    }
                }
            }

            // Commit the offer to the data store.
            if (resultCode == ResultCode.Success || resultCode == ResultCode.Created)
            {
                resultCode = AddOrUpdateOffer(offer);
            }

            response.ResultSummary.ResultCode = resultCode.ToString();
        }

        /// <summary>
        /// Builds a Provider object from the contents of the specified V3DealDataContract.
        /// </summary>
        /// <param name="deal">
        /// The V3DealDataContract from which to build the Provider object.
        /// </param>
        /// <param name="provider">
        /// Receives the Provider object that was built.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode BuildProvider(V3DealDataContract deal,
                                         out Provider provider)
        {
            provider = new Provider
            {
                GlobalID = deal.ProviderId,
                Name = deal.ProviderName
            };

            return ResultCode.Success;
        }

        /// <summary>
        /// Builds a Merchant object from the contents of the specified V3DiscountDataContract.
        /// </summary>
        /// <param name="discount">
        /// The V3DiscountDataContract from which to build the Provider object.
        /// </param>
        /// <param name="globalProviderID">
        /// The ID from the wider services space for the provider from which the merchant was sourced.
        /// </param>
        /// <param name="isNational">
        /// Specifies whether the deal is for a national, as opposed to a local, provider.
        /// </param>
        /// <param name="merchants">
        /// The list of Merchant objects created within this ingestion call.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode BuildMerchant(V3DiscountDataContract discount,
                                         string globalProviderID,
                                         bool isNational,
                                         List<Merchant> merchants)
        {
            ResultCode result = ResultCode.Success;

            // Get merchant info from top-level discount info.
            Merchant merchant = new Merchant
            {
                GlobalID = discount.MerchantId,
                Name = discount.MerchantName,
                GlobalProviderID = globalProviderID,
                IncludePartnerMerchantIDs = true // Don't worry about this one. Entirely internally facing.
            };

            // Then build partner merchant info objects from the partnerMerchantAuthorizationIDs IEnumerable.
            List<PartnerMerchantAuthorizationID> partnerMerchantAuthorizationIDs = new List<PartnerMerchantAuthorizationID>();
            List<PartnerMerchantSettlementID> partnerMerchantSettlementIDs = new List<PartnerMerchantSettlementID>();
            foreach (DiscountPartnerMerchantIds item in discount.PartnerMerchantIds)
            {
                // Get the Partner. NOTE: This will not work for American Express because it's defined in CardBrand as AmericanExpress but in Partner as Amex.
                CardBrand partner = (CardBrand)Enum.Parse(typeof(CardBrand), item.Partner);

                // Populate the MIDs.
                foreach (string idString in item.MerchantIds)
                {
                    string[] elements = idString.Split(';');
                    bool addOrUpdate = elements[elements.Length - 1] == "1";
                    PartnerMerchantAuthorizationID partnerMerchantAuthorizationID = null;
                    PartnerMerchantSettlementID partnerMerchantSettlementID = null;

                    if (partner == CardBrand.Visa)
                    {
                        string id = null;

                        // For national providers (e.g. Whole Foods), both authorization and settlement use just the VMID.
                        if (isNational == true)
                        {
//TODO: What do we do with the vsid?
                            id = elements[0];
                        }
                        // For aggregate providers ("Locals"), Visa's auth and settlement each use both the VMID and the VSID.
                        else
                        {
                            id = String.Concat(elements[0], ';', elements[1]);
                        }

                        partnerMerchantAuthorizationID = new PartnerMerchantAuthorizationID
                        {
                            Partner = partner,
                            AuthorizationID = id,
                            AddOrUpdate = addOrUpdate
                        };
                        
                        partnerMerchantSettlementID = new PartnerMerchantSettlementID
                        {
                            Partner = partner,
                            SettlementID = id,
                            AddOrUpdate = addOrUpdate
                        };
                    }
                    else if (partner == CardBrand.MasterCard)
                    {
                        // MasterCard's auth and settlement IDs are different. "<acquirerIca>;<merchantId;<locationId>" is split into "<acquirerIca>;<merchantId>"
                        //  and "<locationId>". The former is the auth ID, the latter the settlement ID. But not all portions will always appear in what we're sent.
                        if (String.IsNullOrWhiteSpace(elements[0]) == false && String.IsNullOrWhiteSpace(elements[1]) == false)
                        {
                            partnerMerchantAuthorizationID = new PartnerMerchantAuthorizationID
                            {
                                Partner = partner,
                                AuthorizationID = String.Concat(elements[0], ';', elements[1]),
                                AddOrUpdate = addOrUpdate
                            };
                        }

                        if (String.IsNullOrWhiteSpace(elements[2]) == false)
                        {
                            partnerMerchantSettlementID = new PartnerMerchantSettlementID
                            {
                                Partner = partner,
                                SettlementID = elements[2],
                                AddOrUpdate = addOrUpdate
                            };
                        }
                    }

                    if (partnerMerchantAuthorizationID != null)
                    {
                        partnerMerchantAuthorizationIDs.Add(partnerMerchantAuthorizationID);
                    }

                    if (partnerMerchantSettlementID != null)
                    {
                        partnerMerchantSettlementIDs.Add(partnerMerchantSettlementID);
                    }
                }
            }

            if (result == ResultCode.Success)
            {
                merchant.PartnerMerchantAuthorizationIDs = partnerMerchantAuthorizationIDs;
                merchant.PartnerMerchantSettlementIDs = partnerMerchantSettlementIDs;
                merchants.Add(merchant);
            }

            return result;
        }

        /// <summary>
        /// Builds an Offer object from the contents of the specified V3DealDataContract.
        /// </summary>
        /// <param name="deal">
        /// The V3DealDataContract from which to build the Offer object.
        /// </param>
        /// <param name="offer">
        /// Receives the Offer object that was built.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode BuildOffer(V3DealDataContract deal,
                                      out Offer offer)
        {
            // Flattened Offer data is gathered from the first V3DiscountDataContract instance.
            V3DiscountDataContract generalInfo = deal.Discounts.First();

            // Get the offer type.
            OfferType offerType = OfferType.Earn; // Most are Earn.
            string targetString;
            PopulateString("reimbursement_tender", generalInfo.Properties, out targetString);
            ReimbursementTender reimbursementTender = (ReimbursementTender)Enum.Parse(typeof(ReimbursementTender), targetString, true);
            if (reimbursementTender == ReimbursementTender.MicrosoftBurn)
            {
                offerType = OfferType.Burn;
            }

            // Get the percent back.
            decimal percentBack = 0;
            PopulateDecimal("percent", generalInfo.Properties, out percentBack);

            // Get the active state.
            bool active = DateTime.UtcNow < generalInfo.EndDate;

            offer = new Offer
            {
                GlobalID = deal.Id,
                GlobalProviderID = deal.ProviderId,
                OfferType = offerType,
                PercentBack = percentBack,
                Active = active
            };

            return ResultCode.Success;
        }

        /// <summary>
        /// Adds or updates the specified Provider in the data store.
        /// </summary>
        /// <param name="provider">
        /// The provider to add or update.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode AddOrUpdateProvider(Provider provider)
        {
            ResultCode result = ResultCode.Success;

            Context.Log.Verbose("Attempting to add or update the provider within the data store.");
            Context[Key.Provider] = provider;
            result = ProviderOperations.AddOrUpdateProvider();
            Context.Log.Verbose("ResultCode after adding or updating the provider within the data store: {0}", result);

            return result;
        }

        /// <summary>
        /// Adds or updates the specified Merchant in the data store.
        /// </summary>
        /// <param name="merchant">
        /// The merchant to add or update.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode AddOrUpdateMerchant(Merchant merchant)
        {
            ResultCode result = ResultCode.Success;

            Context.Log.Verbose("Attempting to add or update the merchant within the data store.");
            Context[Key.Merchant] = merchant;
            result = MerchantOperations.AddOrUpdateMerchant();
            Context.Log.Verbose("ResultCode after adding or updating the merchant within the data store: {0}", result);

            return result;
        }

        /// <summary>
        /// Adds or updates the specified Offer in the data store.
        /// </summary>
        /// <param name="offer">
        /// The offer to add or update.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode AddOrUpdateOffer(Offer offer)
        {
            ResultCode result = ResultCode.Success;

            Context.Log.Verbose("Attempting to add or update the offer within the data store.");
            Context[Key.Offer] = offer;
            result = OfferOperations.AddOrUpdateOffer();
            Context.Log.Verbose("ResultCode after adding or updating the offer within the data store: {0}", result);

            return result;
        }

        /// <summary>
        /// Populates a string with the value under the specifed key in the specified property bag.
        /// </summary>
        /// <param name="key">
        /// The key under which the value to populate can be found within the property bag.
        /// </param>
        /// <param name="properties">
        /// The property bag containing the value to populate within the string.
        /// </param>
        /// <param name="target">
        /// The string to populate with the value under the specified key in the property bag.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        internal static ResultCode PopulateString(string key,
                                                  IDictionary<string, string> properties,
                                                  out string target)
        {
            ResultCode result = ResultCode.Success;

            if (properties.ContainsKey(key) == true)
            {
                target = properties[key];
            }
            else
            {
                result = ResultCode.InvalidParameter;
                target = null;
            }

            return result;
        }

        /// <summary>
        /// Populates a decimal with the value under the specifed key in the specified property bag.
        /// </summary>
        /// <param name="key">
        /// The key under which the value to populate can be found within the property bag.
        /// </param>
        /// <param name="properties">
        /// The property bag containing the value to populate within the decimal.
        /// </param>
        /// <param name="target">
        /// The decimal to populate with the value under the specified key in the property bag.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        internal static ResultCode PopulateDecimal(string key,
                                                   IDictionary<string, string> properties,
                                                   out decimal target)
        {
            ResultCode result = ResultCode.Success;

            if (properties.ContainsKey(key) == true)
            {
                if (Decimal.TryParse(properties[key], out target) == false)
                {
                    result = ResultCode.InvalidParameter;
                }
            }
            else
            {
                result = ResultCode.InvalidParameter;
                target = 0;
            }

            return result;
        }
/*
        /// <summary>
        /// Executes the Register deal API invocation.
        /// </summary>
        /// <param name="generatePartnerDealId">
        /// The function to call to generate the partner deal ID.
        /// </param>
        /// <param name="registerDealInvoker">
        /// The function to call to invoke registration of the deal with partners.
        /// </param>
        /// <returns>
        /// A task which will perform the work in this method.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
         Justification = "Sending in Funcs prevents circular assembly dependency. " +
                         "Code refactoring to flatten Core and Executor assemblies into a single assembly " +
                         "and remove the Concluder concept is pending.")]
        public async Task Execute(Func<PartnerDealInfo, CommerceContext, string> generatePartnerDealId,
                                  Func<Task<ResultCode>> registerDealInvoker)
        {
            try
            {
                ResultCode resultCode = ResultCode.Success;

                V3RegisterDealResponse response = (V3RegisterDealResponse)Context[Key.Response];
                response.DiscountResults = new Dictionary<Guid, string>();

                if (DealObjectReceived() == true)
                {
                    V3DealDataContract v3DealDataContract = (V3DealDataContract)Context[Key.DealDataContract];
                    foreach (V3DiscountDataContract discountDataContract in v3DealDataContract.Discounts)
                    {
                        DealDataContract dealDataContract = new DealDataContract();
                        resultCode = SharedDealLogic.PopulateDealDataContract(dealDataContract, discountDataContract);
                        if (resultCode == ResultCode.Success)
                        {
                            if (SharedDealLogic.DealMayBeValid(dealDataContract) == true)
                            {
                                resultCode = PopulateNewDealOrOverrideExistingDeal(dealDataContract, v3DealDataContract,
                                                                                   discountDataContract, generatePartnerDealId);

                                // If the new deal could be created, register it with partners.
                                if (resultCode == ResultCode.Success)
                                {
                                    Context.Log.Verbose("Attempting to register the deal with the appropriate partner(s).");
                                    resultCode = registerDealInvoker().Result;
                                }

                                if (resultCode == ResultCode.Success)
                                {
                                    // if this is a new deal, schedule auto link job when needed
                                    Deal deal = ((Deal)Context[Key.Deal]);
                                    if (SharedDealLogic.AllPartnersRegistered(deal))
                                    {
                                        // Only schedule Auto-linking for non-Earn deals that contain First Data MIDs.
                                        if ((deal.ReimbursementTender & ReimbursementTender.MicrosoftEarn) != ReimbursementTender.MicrosoftEarn &&
                                            deal.PartnerDealInfoList.Any(info => info.PartnerId == Partner.FirstData) == true)
                                        {
                                            // If deal registration is still pending
                                            if (deal.DealStatusId == DealStatus.PendingRegistration)
                                            {
                                                // mark pending autolinking
                                                deal.DealStatusId = DealStatus.PendingAutoLinking;
                                                RegisterDeal();
                                                // schedule autolinking
                                                await ScheduleAutoLinkingAsync().ConfigureAwait(false);

                                                resultCode = ResultCode.JobQueued;
                                            }
                                            else
                                            {
                                                // in other cases - update, still persist the deal
                                                RegisterDeal();
                                            }
                                        }
                                        else
                                        {
                                            deal.DealStatusId = DealStatus.Activated;
                                            RegisterDeal();
                                        }
                                    }
                                }
                                else if (resultCode == ResultCode.JobQueued)
                                {
                                    // persist the deal right now, with the current status so we don't lose the changes.
                                    // we will update it if needed later
                                    RegisterDeal();
                                    // Some batch ops are to be done. 
                                    // After completion of those ops, deal will be updated with registeration status
                                    // and auto linking done.
                                }
                            }
                            else
                            {
                                resultCode = ResultCode.InvalidDeal;
                            }
                        }
                        
                        // Store the result code for this discount.
                        response.DiscountResults[discountDataContract.Id] = resultCode.ToString();
                    }

                    // Determine overall result from individual results.
                    resultCode = ResultCode.Success;
                    foreach (Guid key in response.DiscountResults.Keys)
                    {
                        string discountResult = response.DiscountResults[key];
                        if (discountResult == ResultCode.JobQueued.ToString())
                        {
                            resultCode = ResultCode.JobQueued;
                        }
                        else if (discountResult != ResultCode.Created.ToString() && discountResult != ResultCode.Success.ToString())
                        {
                            resultCode = ResultCode.AggregateError;
                        }
                    }

                }
                else
                {
                    resultCode = ResultCode.ParameterCannotBeNull;
                }

                // Build the response and send it back to the caller.
                response.ResultSummary.SetResultCode(resultCode);
                RestResponder.BuildAsynchronousResponse(Context);
            }
            catch (Exception ex)
            {
                ((ResultSummary)Context[Key.ResultSummary]).SetResultCode(ResultCode.UnknownError);
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

*/

        /// <summary>
        /// Retrieves the Deal with the specified DealId from the data store and logs accordingly.
        /// </summary>
        /// <returns>
        /// * The Deal with the specified ID if successful.
        /// * Else returns null.
        /// </returns>
        public Deal RetrieveDeal()
        {
            Deal result;

            Guid dealId = (Guid)Context[Key.GlobalDealId];
            Context.Log.Verbose("Attempting to retrieve a Deal with ID {0} from the data store.", dealId);
            result = DealOperations.RetrieveDeal();
            if (result != null)
            {
                Context.Log.Verbose("Deal retrieved successfully.");
            }
            else
            {
                Context.Log.Verbose("No Deal with ID {0} found.", dealId);
            }

            return result;
        }

        /// <summary>
        /// Registers the deal in the context, updating if deal already existed.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public ResultCode RegisterDeal()
        {
// This gets called by the ClaimDiscountForExistingCardsJob to mark the Deal as completely ingested. The new Offers schema has no such concept.
// So this is a no-op. It along with everything related to it can be removed when First Data is no longer needed.

            ResultCode result = ResultCode.Success;

            //// Register the deal.
            //if ((bool)Context[Key.PreviouslyRegistered] == false)
            //{
            //    Context.Log.Verbose("Attempting to register the deal within the data store.");
            //    result = DealOperations.RegisterDeal();
            //    Context.Log.Verbose("ResultCode after registering the deal within the data store: {0}", result);
            //}
            //else
            //{
            //    Context.Log.Verbose("Determining whether Deal has been changed.");
            //    if (((Deal)Context[Key.Deal]).Equals((Deal)Context[Key.InitialDeal]) == false)
            //    {
            //        Context.Log.Verbose("Deal has been changed. Attempting to update the deal in the data store.");
            //        result = DealOperations.UpdateDeal();
            //        Context.Log.Verbose("ResultCode after updating the deal in the data store: {0}", result);
            //    }
            //    else
            //    {
            //        result = ResultCode.Success;
            //        Context.Log.Verbose("No changes to the Deal occurred. Data store will not be altered.");
            //    }
            //}

            return result;
        }
/*
        /// <summary>
        /// Performs preliminary checks to determine whether the deal may be valid.
        /// </summary>
        /// <param name="dealDataContract">
        /// The DealDataContract whose validity to check.
        /// </param>

        /// <returns>
        /// * True if the deal may be valid.
        /// * Else returns false.
        /// </returns>
        /// <remarks>
        /// This method does not determine whether the deal actually _is_ valid, i.e. whether it corresponds to a deal in
        /// PublishedDeals, whether the discount amount specified is in any way accurate, etc.. Instead, it determines whether
        /// the deal _may be_ valid, by ensuring values are possible and correct relative to one another.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Parameter dealDataContract cannot be null.
        /// </exception>
        public static bool DealMayBeValid(DealDataContract dealDataContract)
        {
            if (dealDataContract == null)
            {
                throw new ArgumentNullException("dealDataContract", "Parameter dealDataContract cannot be null.");
            }

            bool result = false;

            // Apply default value to Deal.EndDate prior to validation.
            if (dealDataContract.EndDate == DateTime.MinValue)
            {
                dealDataContract.EndDate = DateTime.MaxValue;
            }

            if (dealDataContract.Id != Guid.Empty &&
                dealDataContract.StartDate < dealDataContract.EndDate &&
                dealDataContract.Amount >= 0 &&
                dealDataContract.Percent >= 0.0M &&
                dealDataContract.Percent <= 100.0M &&
                dealDataContract.MinimumPurchase >= 0 &&
                (dealDataContract.MinimumPurchase == 0 || dealDataContract.MinimumPurchase >= dealDataContract.Amount) &&
                dealDataContract.Count >= 0 &&
                dealDataContract.UserLimit >= 0 &&
                (dealDataContract.Count == 0 || dealDataContract.Count >= dealDataContract.UserLimit))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Determines if a deal object was received and logs accordingly.
        /// </summary>
        /// <returns>
        /// * True if the deal object is not null.
        /// * Else returns false.
        /// </returns>
        public bool DealObjectReceived()
        {
            bool result = Context[Key.DealDataContract] != null;

            Context.Log.Verbose("Determining if DealDataContract object is null.");
            if (result == true)
            {
                Context.Log.Verbose("DealDataContract object is not null.");
            }
            else
            {
                Context.Log.Verbose("DealDataContract object is null.");
            }

            return result;
        }

        /// <summary>
        /// Populates the Deal in the context from DealDataContract contents.
        /// </summary>
        /// <param name="dealDataContract">
        /// The DealDataContract from which the Deal will be populated.
        /// </param>
        /// <param name="v3DealDataContract">
        /// The v3DealDataContract to use when populating the Deal.
        /// </param>
        /// <param name="discountDataContract">
        /// The discount data contract from which the deal is being populated.
        /// </param>
        /// <returns>
        /// * ResultCode.Success if populating the deal was successful.
        /// * Else other ResultCodes.
        /// </returns>
        internal ResultCode PopulateDeal(DealDataContract dealDataContract,
                                         V3DealDataContract v3DealDataContract,
                                         V3DiscountDataContract discountDataContract)
        {
            ResultCode result = ResultCode.Success;

            Context.Log.Verbose("Populating Deal from DiscountDataContract.");
            
            ReimbursementTender reimbursementTender = ReimbursementTender.DealCurrency;

            if (String.IsNullOrWhiteSpace(dealDataContract.ReimbursementTender) == false &&
                Enum.TryParse<ReimbursementTender>(dealDataContract.ReimbursementTender, out reimbursementTender) == false)
            {
                result = ResultCode.InvalidParameter;
            }
            else
            {
                Deal deal = (Deal)Context[Key.Deal];
                deal.GlobalId = dealDataContract.Id;
                deal.ParentDealId = v3DealDataContract.Id;
                deal.ProviderId = v3DealDataContract.ProviderId;
                deal.ProviderCategory = v3DealDataContract.ProviderCategory;
                deal.StartDate = dealDataContract.StartDate;
                deal.EndDate = dealDataContract.EndDate;
                deal.Currency = dealDataContract.Currency;
                deal.ReimbursementTender = reimbursementTender;
                deal.Amount = dealDataContract.Amount;
                deal.Percent = dealDataContract.Percent;
                deal.MinimumPurchase = dealDataContract.MinimumPurchase;
                deal.Count = dealDataContract.Count;
                deal.UserLimit = dealDataContract.UserLimit;
                deal.DiscountSummary = dealDataContract.DiscountSummary;
                if (discountDataContract != null)
                {
                    deal.SetDayTimeRestrictions(discountDataContract.DayTimeRestrictions);
                }
                if (dealDataContract.Percent > 0M)
                {
                    deal.MaximumDiscount = dealDataContract.MaximumDiscount;
                }
                else
                {
                    deal.MaximumDiscount = dealDataContract.Amount;
                }

                // Determine which PartnerDealInfo objects have already been added to the deal.
                Dictionary<Partner, PartnerDealInfo> partnerDeals = new Dictionary<Partner, PartnerDealInfo>();
                foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
                {
                    partnerDeals[partnerDealInfo.PartnerId] = partnerDealInfo;
                    partnerDealInfo.PartnerMerchantLocations.Clear();
                }

                // Lookup partner merchant IDs.
                result = PopulatePartnerMerchantIds(deal, ((V3DealDataContract)v3DealDataContract).ProviderCategory, partnerDeals, discountDataContract);
            }

            return result;
        }

        /// <summary>
        /// Populate the new deal or overrride the data points of an existing deal
        /// </summary>
        /// <param name="dealDataContract">
        /// Deal Data Contract
        /// </param>
        /// <param name="v3DealDataContract">
        /// Incoming V3 Deal contract
        /// </param>
        /// <param name="discountDataContract">
        /// The discount data contract from which the deal is being populated.
        /// </param>
        /// <param name="generatePartnerDealId">
        /// The function to call to generate the partner deal ID.
        /// </param>
        /// <returns>
        /// Result Code
        /// </returns>
        internal ResultCode PopulateNewDealOrOverrideExistingDeal(DealDataContract dealDataContract,
                                                                  V3DealDataContract v3DealDataContract,
                                                                  V3DiscountDataContract discountDataContract,
                                                                  Func<PartnerDealInfo, CommerceContext, string> generatePartnerDealId)
        {
            ResultCode resultCode;

            Context[Key.GlobalDealId] = dealDataContract.Id;
            Deal deal = RetrieveDeal();
            bool previouslyRegistered = deal != null;
            Context[Key.PreviouslyRegistered] = previouslyRegistered;
            if (previouslyRegistered == true)
            {
                Context[Key.Deal] = deal;
                Context[Key.InitialDeal] = new Deal(deal);
                resultCode = PopulateDeal(dealDataContract, v3DealDataContract, discountDataContract);
                // for each new partner, set a pending status
                foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
                {
                    if (String.IsNullOrWhiteSpace(partnerDealInfo.PartnerDealId) == true)
                    {
                        partnerDealInfo.PartnerDealId = generatePartnerDealId(partnerDealInfo, Context);
                    }

                    if (partnerDealInfo.PartnerDealRegistrationStatusId == PartnerDealRegistrationStatus.None)
                    {
                        partnerDealInfo.PartnerDealRegistrationStatusId = PartnerDealRegistrationStatus.Pending;
                    }
                }
            }
            else
            {
                deal = new Deal();
                Context[Key.Deal] = deal;
                resultCode = PopulateDeal(dealDataContract, v3DealDataContract, discountDataContract);
                // for each partner set a pending status
                foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
                {
                    partnerDealInfo.PartnerDealId = generatePartnerDealId(partnerDealInfo, Context);
                    partnerDealInfo.PartnerDealRegistrationStatusId = PartnerDealRegistrationStatus.Pending;
                }

                // for the deal on the whole, set pending status
                deal.DealStatusId = DealStatus.PendingRegistration;

                // this will persist the deal to DB
                RegisterDeal();
            }

            return resultCode;
        }

        /// <summary>
        /// Schedule a deal to be autolinked to all the valid cards
        /// </summary>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        internal async Task ScheduleAutoLinkingAsync()
        {
            ConcurrentDictionary<string, string> payload = new ConcurrentDictionary<string, string>();
            payload[Key.GlobalDealId.ToString()] = ((Deal) Context[Key.Deal]).GlobalId.ToString();
            ScheduledJobDetails scheduledJobDetails = new ScheduledJobDetails
            {
                JobId = Guid.NewGuid(),
                JobType = ScheduledJobType.ClaimDiscountForExistingCards,
                Orchestrated = true,
                StartTime = DateTime.UtcNow,
                Payload = payload
            };
            IScheduler scheduler = PartnerFactory.Scheduler(CommerceServiceConfig.Instance.SchedulerQueueName,
                CommerceServiceConfig.Instance.SchedulerTableName,
                CommerceServiceConfig.Instance);
            await scheduler.ScheduleJobAsync(scheduledJobDetails).ConfigureAwait(false);
        }

        /// <summary>
        /// Find whethere the deal is registered with all partners
        /// </summary>
        /// <param name="deal">
        /// Deal 
        /// </param>
        /// <returns>
        /// True/False 
        /// </returns>
        internal static bool AllPartnersRegistered(Deal deal)
        {
            bool result = true;
            foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
            {
               if (partnerDealInfo.PartnerDealRegistrationStatusId != PartnerDealRegistrationStatus.Complete)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        
        /// <summary>
        /// Populates the specified DealDataContract with values from the specified discountDataContract.
        /// </summary>
        /// <param name="dealDataContract">
        /// The DealDataContract object to populate.
        /// </param>
        /// <param name="discountDataContract">
        /// The DiscountDataContract containing values to populate within the DealDataContract.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        internal static ResultCode PopulateDealDataContract(DealDataContract dealDataContract,
                                                            V3DiscountDataContract discountDataContract)
        {
            ResultCode result = ResultCode.Success;

//TODO: Remove this when ingestion system has been fully updated.
            if (discountDataContract.Properties.ContainsKey("reimbursement_tender") == false)
            {
                discountDataContract.Properties["reimbursement_tender"] = ReimbursementTender.DealCurrency.ToString();
            }

            // Populate formal parameters.
            dealDataContract.Id = discountDataContract.Id;
            dealDataContract.StartDate = discountDataContract.StartDate;
            dealDataContract.EndDate = discountDataContract.EndDate;
            dealDataContract.DiscountSummary = discountDataContract.DiscountSummary;

            // Populate common property bag parameters.
            string targetString;
            int targetInt = 0;
            result = PopulateString("currency", discountDataContract.Properties, out targetString);
            if (result == ResultCode.Success)
            {
                dealDataContract.Currency = targetString;
                result = PopulateString("reimbursement_tender", discountDataContract.Properties, out targetString);
            }
            if (result == ResultCode.Success)
            {
                dealDataContract.ReimbursementTender = targetString;
                result = PopulateInt("minimum_purchase", discountDataContract.Properties, out targetInt);
            }
            if (result == ResultCode.Success)
            {
                dealDataContract.MinimumPurchase = targetInt;
                result = PopulateInt("redemption_limit", discountDataContract.Properties, out targetInt);
            }
            if (result == ResultCode.Success)
            {
                dealDataContract.Count = targetInt;
                result = PopulateInt("user_redemption_limit", discountDataContract.Properties, out targetInt);
            }
            if (result == ResultCode.Success)
            {
                dealDataContract.UserLimit = targetInt;

                // Populate type-specific property bag parameters.
                DiscountType discountType;
                if (Enum.TryParse<DiscountType>(discountDataContract.DiscountType, out discountType) == true)
                {
                    switch (discountType)
                    {
                        case DiscountType.StaticStatementCredit:
                            result = PopulateStaticStatementCredit(dealDataContract, discountDataContract.Properties);
                            break;
                        case DiscountType.PercentageStatementCredit:
                            result = PopulatePercentageStatementCredit(dealDataContract, discountDataContract.Properties);
                            break;
                    }
                }
                else
                {
                    result = ResultCode.InvalidParameter;
                }
            }

            return result;
        }

        /// <summary>
        /// Populates the specified DealDataContract with static statement credit-specified values from the specified property
        /// bag.
        /// </summary>
        /// <param name="dealDataContract">
        /// The DealDataContract object to populate.
        /// </param>
        /// <param name="properties">
        /// The property bag containing values to populate within the DealDataContract.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        internal static ResultCode PopulateStaticStatementCredit(DealDataContract dealDataContract,
                                                                 IDictionary<string, string> properties)
        {
            ResultCode result = ResultCode.Success;

            int targetInt = 0;
            result = PopulateInt("amount", properties, out targetInt);
            if (result == ResultCode.Success)
            {
                dealDataContract.Amount = targetInt;
            }

            return result;
        }

        /// <summary>
        /// Populates the specified DealDataContract with percentage statement credit-specified values from the specified
        /// property bag.
        /// </summary>
        /// <param name="dealDataContract">
        /// The DealDataContract object to populate.
        /// </param>
        /// <param name="properties">
        /// The property bag containing values to populate within the DealDataContract.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        internal static ResultCode PopulatePercentageStatementCredit(DealDataContract dealDataContract,
                                                                   IDictionary<string, string> properties)
        {
            ResultCode result = ResultCode.Success;

            decimal targetDecimal;
            int targetInt = 0;
            result = PopulateDecimal("percent", properties, out targetDecimal);
            if (result == ResultCode.Success)
            {
                dealDataContract.Percent = targetDecimal;
                result = PopulateInt("maximum_discount", properties, out targetInt);
            }
            if (result == ResultCode.Success)
            {
                dealDataContract.MaximumDiscount = targetInt;
            }

            return result;
        }

        /// <summary>
        /// Populates an int with the value under the specifed key in the specified property bag.
        /// </summary>
        /// <param name="key">
        /// The key under which the value to populate can be found within the property bag.
        /// </param>
        /// <param name="properties">
        /// The property bag containing the value to populate within the int.
        /// </param>
        /// <param name="target">
        /// The int to populate with the value under the specified key in the property bag.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        internal static ResultCode PopulateInt(string key,
                                               IDictionary<string, string> properties,
                                               out int target)
        {
            ResultCode result = ResultCode.Success;

            if (properties.ContainsKey(key) == true)
            {
                if (Int32.TryParse(properties[key], out target) == false)
                {
                    result = ResultCode.InvalidParameter;
                }
            }
            else
            {
                result = ResultCode.InvalidParameter;
                target = 0;
            }

            return result;
        }

        /// <summary>
        /// Populates the partner merchant IDs into the specified deal for data contained within the specified V3DealDataContract.
        /// </summary>
        /// <param name="deal">
        /// The deal to populate.
        /// </param>
        /// <param name="partnerDeals">
        /// The dictionary of added partner deal info objects.
        /// </param>
        /// <param name="merchantName">
        /// The name of the merchant whose deal is being registered.
        /// </param>
        /// <param name="discountDataContract">
        /// The discount data contract from which the deal is being populated.
        /// </param>
        /// <returns>
        /// * ResultCode.Success if populating the deal was successful.
        /// * Else other ResultCodes.
        /// </returns>
        private ResultCode PopulatePartnerMerchantIds(Deal deal,
                                                      string providerCategory,
                                                      Dictionary<Partner, PartnerDealInfo> partnerDeals,
                                                      V3DiscountDataContract discountDataContract)
        {
            ResultCode result = ResultCode.Success;

            deal.MerchantName = discountDataContract.MerchantName;
            deal.MerchantId = discountDataContract.MerchantId;
            deal.ProviderCategory = providerCategory;
            Context[Key.MerchantName] = discountDataContract.MerchantName;

            // Add new PartnerDealInfo objects.
            foreach (DiscountPartnerMerchantIds partnerMerchantIds in discountDataContract.PartnerMerchantIds)
            {
                // Determine the Partner from the ExternalSource. These won't maintain a 1:1 correlation in future releases.
                Partner partner;
                if (Enum.TryParse<Partner>(partnerMerchantIds.Partner, out partner) == true)
                {
                    // If a PartnerDealInfo object hasn't already been created for the Partner, create one.
                    if (partnerDeals.ContainsKey(partner) == false)
                    {
                        PartnerDealInfo partnerDealInfo = new PartnerDealInfo { PartnerId = partner };
                        partnerDeals[partner] = partnerDealInfo;
                        deal.PartnerDealInfoList.Add(partnerDealInfo);
                    }

                    // Add the ExternalMerchantId to the PartnerDealInfo.
                    foreach (string partnerMerchantId in partnerMerchantIds.MerchantIds)
                    {
                        // If this is a complex partner merchant ID, populate from its components.
                        ComplexPartnerMerchantId complexId;
                        try
                        {
                            complexId = General.DeserializeJson<ComplexPartnerMerchantId>(partnerMerchantId);
                        }
                        catch(JsonReaderException)
                        {
                            complexId = null;
                        }
                        catch(JsonSerializationException)
                        {
                            complexId = null;
                        }
                        if (complexId != null)
                        {
                            if (complexId.TimeZoneId != null)
                            {
                                try
                                {
                                    TimeZoneInfo.FindSystemTimeZoneById(complexId.TimeZoneId);
                                }
                                catch (TimeZoneNotFoundException)
                                {
                                    result = ResultCode.InvalidParameter;
                                    break;
                                }
                            }

                            PartnerMerchantIdType partnerMerchantIdType = PartnerMerchantIdType.Universal;
                            if (complexId.IdType == null || Enum.TryParse<PartnerMerchantIdType>(complexId.IdType, out partnerMerchantIdType) == false)
                            {
                                result = ResultCode.InvalidParameter;
                                break;
                            }

                            partnerDeals[partner].PartnerMerchantLocations.Add(new PartnerMerchantLocationInfo
                            {
                                PartnerMerchantId = complexId.MerchantId,
                                PartnerMerchantIdType = partnerMerchantIdType,
                                MerchantTimeZoneId = complexId.TimeZoneId
                            });
                        }
                        // Otherwise, take the specified ID wholesale.
                        else
                        {
                            partnerDeals[partner].PartnerMerchantLocations.Add(new PartnerMerchantLocationInfo
                            {
                                PartnerMerchantId = partnerMerchantId
                            });
                        }
                    }
                }
                else
                {
                    result = ResultCode.InvalidParameter;
                    break;
                }
            }

            // Remove any partners that did not contain partner merchant IDs after processing.
            foreach (Partner populatedPartner in partnerDeals.Keys)
            {
                if (partnerDeals[populatedPartner].PartnerMerchantLocations.Count == 0)
                {
                    PartnerDealInfo removePartnerDealInfo = deal.PartnerDealInfoList.First((partnerDealInfo) => partnerDealInfo.PartnerId == populatedPartner);
                    deal.PartnerDealInfoList.Remove(removePartnerDealInfo);
                }
            }

            return result;
        }
*/
        /// <summary>
        /// Gets or sets the context of the current API call.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the object to use to perform operations on deals.
        /// </summary>
        private IDealOperations DealOperations { get; set; }
 
        /// <summary>
        /// Gets or sets the object to use to perform operations on providers.
        /// </summary>
        private ProviderOperations ProviderOperations { get; set; }
 
        /// <summary>
        /// Gets or sets the object to use to perform operations on offers.
        /// </summary>
        private OfferOperations OfferOperations { get; set; }
 
        /// <summary>
        /// Gets or sets the object to use to perform operations on merchants.
        /// </summary>
        private MerchantOperations MerchantOperations { get; set; }
     }
}