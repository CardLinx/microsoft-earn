//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.DataContract;
using Earn.Offers.Earn.Attributes;
using Earn.Offers.Earn.Helper;
using Earn.Offers.Earn.Models;
using Earn.Offers.Earn.Services;
using Lomo.Commerce.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LoMo.UserServices.DataContract;
using Newtonsoft.Json;

namespace Earn.Offers.Earn.Controllers
{
    [RequireHttps(Order = 2)]
    public class AccountController : Controller
    {
        [LocationAttribute]
        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> Index()
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                AuthorizeUserResult userResult = await AuthorizeUser(liveIdAuthResult);
                if (!userResult.Authorized)
                {
                    return userResult.Result;
                }

                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                string state = (HttpContext.Items["state"] as string) ?? "wa";
                Task<List<Deal>> dealTask = DealService.GetDeals(state);
                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                string secureToken = HttpContext.Items["backendtoken"] as string;
                Task<string> earnedAmountTask = CommerceService.GetTotalEarnedAmount(userModel, secureToken);
                string earnedAmount = await earnedAmountTask;
                List<Deal> deals = await dealTask;
                List<Deal> topDeals = DealService.GetTopDeals(state);

                if (deals != null)
                {
                    AccountsPageModel model = new AccountsPageModel
                    {
                        LocalDeals = deals,
                        TopDeals = topDeals,
                        EarnTotal = earnedAmount,
                        UserId = userModel.UserId,
                        Page = "places to earn"
                    };

                    return View("~/offers/earn/views/account/places.cshtml", model);
                }
            }
            catch (Exception e)
            {
            }

            return HandleServerError();
        }

        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> Spending()
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                AuthorizeUserResult userResult = await AuthorizeUser(liveIdAuthResult);
                if (!userResult.Authorized)
                {
                    return userResult.Result;
                }

                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                string state = (HttpContext.Items["state"] as string) ?? "wa";
                string revIpHeader = HttpContext.Request.Headers["X-FD-RevIP"];
                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                string secureToken = HttpContext.Items["backendtoken"] as string;
                Task<string> earnedAmountTask = CommerceService.GetTotalEarnedAmount(userModel, secureToken);
                string earnedAmount = await earnedAmountTask;
                AccountsPageModel model = new AccountsPageModel
                {
                    EarnTotal = earnedAmount,
                    UserId = userModel.UserId
                };

                return View("~/offers/earn/views/account/spending.cshtml", model);
            }
            catch (Exception e)
            {
            }

            return HandleServerError();
        }

        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> History()
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                AuthorizeUserResult userResult = await AuthorizeUser(liveIdAuthResult);
                if (!userResult.Authorized)
                {
                    return userResult.Result;
                }

                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                string state = (HttpContext.Items["state"] as string) ?? "wa";
                string revIpHeader = HttpContext.Request.Headers["X-FD-RevIP"];
                string secureToken = HttpContext.Items["backendtoken"] as string;
                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                GetEarnBurnTransactionHistoryResponse response = await CommerceService.GetTransactionHistory(userModel, secureToken);

                if (response != null && response.RedemptionHistory != null)
                {
                    AccountsPageModel pageModel = new AccountsPageModel
                    {
                        RedemptionHistory = response.RedemptionHistory.Where(x => x.CreditStatus == "CreditGranted").ToList(),
                        EarnTotal = response.CreditBalance,
                        PendingRedemptionHistory = response.RedemptionHistory.Where(x => x.CreditStatus == "AuthorizationReceived" || x.CreditStatus == "ClearingReceived" || x.CreditStatus == "StatementCreditRequested").ToList(),
                        UserId = userModel.UserId,
                        Page = "history"
                    };

                    List<EarnBurnTransactionItemDataContract> removeList = new List<EarnBurnTransactionItemDataContract>();
                    if (pageModel.PendingRedemptionHistory.Count > 0)
                    {

                        foreach (EarnBurnTransactionItemDataContract item in pageModel.PendingRedemptionHistory)
                        {
                            List<EarnBurnTransactionItemDataContract> result = pageModel.RedemptionHistory.Where(x =>
                                x.CardBrand == item.CardBrand &&
                                x.EventDateTime == item.EventDateTime &&
                                x.RedemptionType == item.RedemptionType &&
                                x.MerchantName == item.MerchantName &&
                                x.LastFourDigits == item.LastFourDigits &&
                                x.EventAmount == item.EventAmount &&
                                x.DiscountAmount == item.DiscountAmount).ToList();

                            if (result != null && result.Count > 0)
                            {
                                removeList.AddRange(result);
                            }
                        }

                        if (removeList.Count > 0)
                        {
                            foreach (EarnBurnTransactionItemDataContract item in removeList)
                            {
                                pageModel.PendingRedemptionHistory.RemoveAll(x =>
                                    x.EventDateTime == item.EventDateTime &&
                                    x.EventAmount == item.EventAmount &&
                                    x.RedemptionType == item.RedemptionType &&
                                    x.DiscountAmount == item.DiscountAmount &&
                                    x.LastFourDigits == item.LastFourDigits &&
                                    x.CardBrand == item.CardBrand &&
                                    x.MerchantName == item.MerchantName);
                            }
                        }

                        if (pageModel.PendingRedemptionHistory != null && pageModel.PendingRedemptionHistory.Count > 0)
                        {
                            foreach (var pendingTransaction in pageModel.PendingRedemptionHistory)
                            {
                                if (pendingTransaction.MerchantName.Trim() == "Shell")
                                {
                                    pendingTransaction.DiscountAmount = "TBD";
                                    pendingTransaction.EventAmount = "TBD";
                                }
                            }
                        }
                    }

                    return View("~/offers/earn/views/account/history.cshtml", pageModel);
                }
            }
            catch (Exception e)
            {
            }

            return HandleServerError();
        }

        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> Settings()
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                AuthorizeUserResult userResult = await AuthorizeUser(liveIdAuthResult);
                if (!userResult.Authorized)
                {
                    return userResult.Result;
                }

                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                string secureToken = HttpContext.Items["backendtoken"] as string;

                Task<V2GetCardsResponse> getCardsResponseTask = CommerceService.GetRegisteredCards(secureToken);
                Task<string> earnedAmountTask = CommerceService.GetTotalEarnedAmount(userModel, secureToken);
                User userInfo = await UserService.GetUserInfo(secureToken);
                string earnedAmount = await earnedAmountTask;
                V2GetCardsResponse getCardsResponse = await getCardsResponseTask;

                AccountsPageModel pageModel = new AccountsPageModel
                {
                    EarnTotal = earnedAmount,
                    Cards = getCardsResponse.Cards,
                    UserInfo = userInfo,
                    Page = "settings",
                    UserId = userModel.UserId
                };

                return View("~/offers/earn/views/account/settings.cshtml", pageModel);
            }
            catch (Exception e)
            {
            }

            return HandleServerError();
        }

        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> Referrals()
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                AuthorizeUserResult userResult = await AuthorizeUser(liveIdAuthResult);
                if (!userResult.Authorized)
                {
                    return userResult.Result;
                }

                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                string secureToken = HttpContext.Items["backendtoken"] as string;
                Task<string> earnedAmountTask = CommerceService.GetTotalEarnedAmount(userModel, secureToken);
                Task<string> referralCodeTask = CommerceService.LoadReferralCode(userModel, secureToken);
                Task<List<ReferralCodeReportDataContract>> referralReportTask = CommerceService.LoadReferralReport(userModel, secureToken);

                string earnedAmount = await earnedAmountTask;
                string referralCode = await referralCodeTask;
                List<ReferralCodeReportDataContract> referralReport = await referralReportTask;

                AccountsPageModel pageModel = new AccountsPageModel
                {
                    EarnTotal = earnedAmount,
                    ReferralReport = referralReport,
                    ReferralCode = referralCode,
                    Page = "referrals",
                    UserId = userModel.UserId
                };

                return View("~/offers/earn/views/account/referrals.cshtml", pageModel);

            }
            catch (Exception e)
            {
            }

            return HandleServerError();
        }

        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> Help()
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                AuthorizeUserResult userResult = await AuthorizeUser(liveIdAuthResult);
                if (!userResult.Authorized)
                {
                    return userResult.Result;
                }

                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                string state = (HttpContext.Items["state"] as string) ?? "wa";
                string revIpHeader = HttpContext.Request.Headers["X-FD-RevIP"];
                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                string secureToken = HttpContext.Items["backendtoken"] as string;
                Task<string> earnedAmountTask = CommerceService.GetTotalEarnedAmount(userModel, secureToken);
                User userInfo = await UserService.GetUserInfo(secureToken);
                string earnedAmount = await earnedAmountTask;
                if (userInfo != null)
                {
                    AccountsPageModel pageModel = new AccountsPageModel
                    {
                        UserInfo = userInfo,
                        EarnTotal = earnedAmount,
                        UserId = userModel.UserId
                    };

                    return View("~/offers/earn/views/account/help.cshtml", pageModel);
                }
            }
            catch (Exception e)
            {
            }

            return HandleServerError();
        }

        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> Support()
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                AuthorizeUserResult userResult = await AuthorizeUser(liveIdAuthResult);
                if (!userResult.Authorized)
                {
                    return userResult.Result;
                }

                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                string state = (HttpContext.Items["state"] as string) ?? "wa";
                string revIpHeader = HttpContext.Request.Headers["X-FD-RevIP"];
                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                string secureToken = HttpContext.Items["backendtoken"] as string;
                Task<string> earnedAmountTask = CommerceService.GetTotalEarnedAmount(userModel, secureToken);
                string earnedAmount = await earnedAmountTask;

                AccountsPageModel pageModel = new AccountsPageModel
                {
                    EarnTotal = earnedAmount,
                    UserId = userModel.UserId
                };

                return View("~/offers/earn/views/account/support.cshtml", pageModel);
            }
            catch (Exception e)
            {
            }

            return HandleServerError();
        }

        [LocationAttribute]
        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> Restaurants(DealSortOrder sortBy = DealSortOrder.City, string sortOrder = "asc")
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                AuthorizeUserResult userResult = await AuthorizeUser(liveIdAuthResult);
                if (!userResult.Authorized)
                {
                    return userResult.Result;
                }

                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                string state = (HttpContext.Items["state"] as string) ?? "wa";
                if (state == "wa")
                {
                    ViewBag.wa = "selected";
                }
                else if (state == "az")
                {
                    ViewBag.az = "selected";
                }
                if (state == "ma")
                {
                    ViewBag.ma = "selected";
                }

                string revIpHeader = HttpContext.Request.Headers["X-FD-RevIP"];
                Task<List<Deal>> dealTask = DealService.GetDeals(state);
                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                string secureToken = HttpContext.Items["backendtoken"] as string;
                Task<string> earnedAmountTask = CommerceService.GetTotalEarnedAmount(userModel, secureToken);
                string earnedAmount = await earnedAmountTask;
                List<Deal> deals = await dealTask;
                SortDealsBy(deals, sortBy, sortOrder);

                AccountsPageModel pageModel = new AccountsPageModel
                {
                    LocalDeals = deals,
                    SelectedState = state,
                    EarnTotal = earnedAmount,
                    SortBy = Enum.GetName(sortBy.GetType(), sortBy),
                    SortOrder = sortOrder,
                    Page = "restaurants",
                    UserId = userModel.UserId
                };

                return View("~/offers/earn/views/account/restaurants.cshtml", pageModel);
            }
            catch (Exception e)
            {
            }

            return HandleServerError();
        }

        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> GiftCards()
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                AuthorizeUserResult userResult = await AuthorizeUser(liveIdAuthResult);
                if (!userResult.Authorized)
                {
                    return userResult.Result;
                }

                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                string state = (HttpContext.Items["state"] as string) ?? "wa";
                string revIpHeader = HttpContext.Request.Headers["X-FD-RevIP"];
                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                string secureToken = HttpContext.Items["backendtoken"] as string;
                Task<string> earnedAmountTask = CommerceService.GetTotalEarnedAmount(userModel, secureToken);
                User userInfo = await UserService.GetUserInfo(secureToken);
                string earnedAmount = await earnedAmountTask;
                if (userInfo != null)
                {
                    AccountsPageModel pageModel = new AccountsPageModel
                    {
                        UserInfo = userInfo,
                        EarnTotal = earnedAmount,
                        UserId = userModel.UserId,
                        Page = "GiftCards"
                    };

                    return View("~/offers/earn/views/account/giftcards.cshtml", pageModel);
                }
            }
            catch (Exception e)
            {
            }

            return HandleServerError();
        }


        private void SortDealsBy(List<Deal> deals, DealSortOrder sort, string sortOrder)
        {
            deals.Sort(DealBusinessComparer.GetInstance(sort));

            if (sortOrder == "dsc")
            {
                deals.Reverse();
            }
        }

        private ActionResult HandleUnauthenticatedOrUnregisteredUser(LiveIdAuthResult result)
        {
            LearnPageModel learnPageModel = new LearnPageModel();
            if (result != null)
            {
                learnPageModel.LiveIdResult = result;
            }

            return View("~/offers/earn/views/learn/learn.cshtml", learnPageModel);
        }

        private ActionResult HandleServerError()
        {
            AccountsPageModel model = new AccountsPageModel();
            model.EarnTotal = "0.00";
            return View("~/offers/earn/views/account/accounterror.cshtml", model);
        }

        private async Task<AuthorizeUserResult> AuthorizeUser(LiveIdAuthResult liveIdAuthResult)
        {
            AuthorizeUserResult authorizeResult = new AuthorizeUserResult();

            if (!User.Identity.IsAuthenticated)
            {
                authorizeResult.Result = HandleUnauthenticatedOrUnregisteredUser(liveIdAuthResult);
                return authorizeResult;
            }

            UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
            string secureToken = HttpContext.Items["backendtoken"] as string;
            bool status = await CommerceService.IsUserRegisteredWithCardLink(userModel, secureToken);
            if (!status)
            {
                authorizeResult.Result = HandleUnauthenticatedOrUnregisteredUser(liveIdAuthResult);
                return authorizeResult;
            }

            authorizeResult.Authorized = true;
            return authorizeResult;
        }
    }
}