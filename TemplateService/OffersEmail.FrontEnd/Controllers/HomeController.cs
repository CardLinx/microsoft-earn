//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary></summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.FrontEnd.Controllers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using OffersEmail.Models;
    using Testing;
    using ViewModels;

    /// <summary>
    /// Home controller
    /// </summary>
    public class HomeController : Controller
    {
        #region Public Methods and Operators

        /// <summary>
        /// Index view.
        /// </summary>
        /// <returns>The Index view</returns>
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Test trending deals email
        /// </summary>
        /// <returns>the daily deals email html</returns>
        public async Task<ContentResult> TestTrendingTemplate()
        {
            return await GetTemplate(TestData.GenerateDealsJson(3, DealType.Prepaid), "/GetEmail/Trending");
        }

        /// <summary>
        /// Test Daily deals email
        /// </summary>
        /// <param name="id">the referrer</param>
        /// <returns>the daily deals email html</returns>
        public async Task<ContentResult> TestCLODailyDealsTemplate(string id = "BO_EMAIL")
        {
            return await GetTemplate(TestData.GenerateDealsJson(40), "/GetEmail/DailyDeals/testemailcampaign/" + id);
        }

        /// <summary>
        /// Test MSN deals email
        /// </summary>
        /// <returns>the MSN deals email html</returns>
        public async Task<ContentResult> TestMsnDealsTemplate()
        {
            return await GetTemplate(TestData.GenerateDealsJson(3), "/GetEmail/MsnDeals");
        }

        /// <summary>
        /// Test the MsnDealsIntroTemplate
        /// </summary>
        /// <returns>content of the emsil</returns>
        public async Task<ContentResult> TestMsnDealsIntro1Template()
        {
            return await GetTemplate(TestData.GenerateDealsJson(3), "/GetEmail/MsnDealsIntro1");
        }

        /// <summary>
        /// Test the MsnDealsIntroTemplate
        /// </summary>
        /// <param name="id">The parameter passed that contains the campaign</param>
        /// <returns>content of the emsil</returns>
        public async Task<ContentResult> TestGiftsTemplate(string id = "BO_Email")
        {
            return await GetTemplate(TestData.GenerateDealsJson(5), "/GetEmail/Gifts/testemailcampaign/" + id);
        }

        /// <summary>
        /// Test change terms of use email
        /// </summary>
        /// <returns>the change terms of use email html</returns>
        public async Task<ContentResult> TestChangeTermsTemplate()
        {
            return await GetTemplate(string.Empty, "/GetEmail/ChangeTerms");
        }

        /// <summary>
        /// Test MSN deals email
        /// </summary>
        /// <returns>the MSN deals email html</returns>
        public async Task<ContentResult> TestCardLinkDealsTemplate()
        {
            return await GetTemplate(TestData.GenerateDealsJson(3), "/GetEmail/CardLinkDeals/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the daily deals email html</returns>
        public async Task<ContentResult> TestMerchantReportTemplate()
        {
            return await GetTemplate(TestData.GetMerchantReportVM(), "/GetEmail/MerchantReport");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the daily deals email html</returns>
        public async Task<ContentResult> TestAutoprospectingTemplate()
        {
            return await GetTemplate(TestData.GetAutoprospectingVM(), "/GetEmail/Autoprospecting");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the daily deals email html</returns>
        public async Task<ContentResult> TestAutoprospectingCreateNewOfferTemplate()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateOfferVM("RestaurantAP1"), "/GetEmail/AutoprospectingCreateNewOffer/testemailcampaign/");
        }

        /// <summary>
        /// Tests the autoprospecting deem template.
        /// </summary>
        /// <returns>
        /// The Autoprospecting Deem Template
        /// </returns>
        public async Task<ContentResult> TestAutoprospectingCreateNewOfferDeemTemplate()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateOfferVM("DeemGenericAP1"), "/GetEmail/AutoprospectingCreateNewOffer/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the Ads autoprospecting email html</returns>
        public async Task<ContentResult> TestAutoprospectingBaxWithCoupon()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateAdVM("BAXAP1"), "/GetEmail/AutoprospectingCreateNewAd/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the Ads autoprospecting email html</returns>
        public async Task<ContentResult> TestBAXAPRich2()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateAdVM("BAXAPRich2"), "/GetEmail/AutoprospectingCreateNewAd/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the Ads autoprospecting email html</returns>
        public async Task<ContentResult> TestBAXAPRich2FU()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateAdVM("BAXAPRich2FU"), "/GetEmail/AutoprospectingCreateNewAd/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the BP Claim Template</returns>
        public async Task<ContentResult> TestBPClaimFB1()
        {
            return await GetTemplate(TestData.GetBusinessClaimVM(), "/GetEmail/AutoprospectingCreateNewBusinessClaim/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the Ads autoprospecting email html</returns>
        public async Task<ContentResult> TestBAXFollowupEmail()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateAdVM("AP1_Followup1"), "/GetEmail/AutoprospectingCreateNewAd/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the Ads autoprospecting email html</returns>
        public async Task<ContentResult> TestAutoprospectingBaxRich()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateAdVM("BAXAP2"), "/GetEmail/AutoprospectingCreateNewAd/testemailcampaign/");
        }

        /// <summary>
        /// Test BAX Drop Off mail
        /// </summary>
        /// <returns>the Ads autoprospecting email html</returns>
        public async Task<ContentResult> TestBaxDropOffAdCreated()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateAdVM("BAXDropOff1"), "/GetEmail/AutoprospectingCreateNewAd/testemailcampaign/");
        }

        /// <summary>
        /// Test BAX Drop Off mail
        /// </summary>
        /// <returns>the Ads autoprospecting email html</returns>
        public async Task<ContentResult> TestBaxDropOffAdNotCreated()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateAdVM("BAXDropOff2"), "/GetEmail/AutoprospectingCreateNewAd/testemailcampaign/");
        }

        /// <summary>
        /// Test BAX Drop Off without coupon mail
        /// </summary>
        /// <returns>the Ads autoprospecting email html</returns>
        public async Task<ContentResult> TestBaxDropOffWithoutCoupon()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateAdVM("BAXDropOffWC"), "/GetEmail/AutoprospectingCreateNewAd/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the daily deals email html</returns>
        public async Task<ContentResult> TestAutoprospectingBaxWithoutCoupon()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateAdVM("BAXAP1", string.Empty), "/GetEmail/AutoprospectingCreateNewAd/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting email
        /// </summary>
        /// <returns>the Ads autoprospecting email html</returns>
        public async Task<ContentResult> TestAutoprospectingRuleBasedBaxDropOff()
        {
            return await GetTemplate(TestData.GetAutoprospectingRuleBasedVM("AdExpressUI", "BAXDropOff1", string.Empty), "/GetEmail/AutoprospectingCreateRuleBased/testemailcampaign/");
        }

        /// <summary>
        /// Test autoprospecting invite user email
        /// </summary>
        /// <returns>invite user email html</returns>
        public async Task<ContentResult> TestAutoprospectingInviteUserTemplate()
        {
            return await GetTemplate(TestData.GetAutoprospectingCreateOfferVM("UserInviteAP1"), "/GetEmail/AutoprospectingCreateNewOffer/invitenewuser/");
        }

        /// <summary>
        /// Tests the confirm email update template.
        /// </summary>
        /// <returns>The confirm email update html</returns>
        public async Task<ContentResult> TestConfirmEmailUpdateTemplate()
        {
            return await GetTemplate(TestData.GetConfirmEmail(), "/GetEmail/ConfirmEmailUpdate/testemailcampaign/");
        }

        /// <summary>
        /// Tests the confirm email update text template.
        /// </summary>
        /// <returns>The confirm email text template.</returns>
        public async Task<ContentResult> TestConfirmEmailUpdateTextTemplate()
        {
            return await GetTextTemplate(TestData.GetConfirmEmail(), "/GetEmail/ConfirmEmailUpdate/testemailcampaign/");
        }

        /// <summary>
        /// Tests the html version of unauthenticated CLO sign up confirmation email template.
        /// </summary>
        /// <returns>Returns unauthenticated CLO sign up confirmation email html template</returns>
        public async Task<ContentResult> TestUnauthCLOConfirmEmailHtmlTemplate()
        {
            return await GetTemplate(TestData.GetConfirmEmail(), "/GetEmail/ConfirmUnAuthenticatedSignupEmail/testemailcampaign/");
        }

        /// <summary>
        /// Tests the text version of unauthenticated CLO sign up confirmation email template.
        /// </summary>
        /// <returns>Returns unauthenticated CLO sign up confirmation email text template</returns>
        public async Task<ContentResult> TestUnauthCLOConfirmEmailTextTemplate()
        {
            return await GetTextTemplate(TestData.GetConfirmEmail(), "/GetEmail/ConfirmUnAuthenticatedSignupEmail/testemailcampaign/");
        }

        /// <summary>
        /// Tests the html version of Link your account to MSID FB ID email template.
        /// </summary>
        /// <returns>Returns Link your account invitation email html template</returns>
        public async Task<ContentResult> TestUnAuthenticatedAccountLinkingInviteEmailHtmlTemplate()
        {
            return await GetTemplate(TestData.GetConfirmEmail(), "/GetEmail/UnAuthenticatedAccountLinkingInviteEmail/testemailcampaign/");
        }

        /// <summary>
        /// Tests the text version of Link your account to MSID FB ID email template.
        /// </summary>
        /// <returns>Returns Link your account invitation email text template</returns>
        public async Task<ContentResult> TestUnAuthenticatedAccountLinkingInviteEmailTextTemplate()
        {
            return await GetTextTemplate(TestData.GetConfirmEmail(), "/GetEmail/UnAuthenticatedAccountLinkingInviteEmail/testemailcampaign/");
        }

        /// <summary>
        /// Tests the CardLink sign up template.
        /// </summary>
        /// <returns>The CardLink sign up email template</returns>
        public async Task<ContentResult> TestCardLinkSignUpTemplate()
        {
            return await GetTemplate(TestData.GetCardLink(), "/CardLink/SignUp/testemailcampaign/");
        }

        /// <summary>
        /// Tests the CardLink sign up text template.
        /// </summary>
        /// <returns>The CardLink sign up email text template</returns>
        public async Task<ContentResult> TestCardLinkSignUpTextTemplate()
        {
            return await GetTextTemplate(TestData.GetCardLink(), "/CardLink/SignUp/testemailcampaign/");
        }

        /// <summary>
        /// Tests the CardLink linked template.
        /// </summary>
        /// <returns>The CardLink linked email template</returns>
        public async Task<ContentResult> TestCardLinkLinkedTemplate()
        {
            return await GetTemplate(TestData.GetCardLink(), "/CardLink/Linked/testemailcampaign/");
        }

        /// <summary>
        /// Tests the CardLink linked text template.
        /// </summary>
        /// <returns>The CardLink linked email text template</returns>
        public async Task<ContentResult> TestCardLinkLinkedTextTemplate()
        {
            return await GetTextTemplate(TestData.GetCardLink(), "/CardLink/Linked/testemailcampaign/");
        }

        /// <summary>
        /// Tests the CardLink auth template.
        /// </summary>
        /// <returns>The CardLink auth email template</returns>
        public async Task<ContentResult> TestCardLinkAuthTemplate()
        {
            return await GetCardLinkAuthOrSettleTemplate("Auth", TestData.GetCardLink());
        }

        public async Task<ContentResult> TestWelcomeToEarnTemplate()
        {
            return await GetTemplate(new OffersEmail.DataContracts.CampaignDataContract(), "/earn/WelcomeToEarn");
        }

        public async Task<ContentResult> TestIfUserHasEarnedTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/IfUserHasEarned");
        }

        public async Task<ContentResult> TestIfUserHasNotEarnedTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/IfUserHasNotEarned");
        }

        /// <summary>
        /// Tests the Newsletter template.
        /// </summary>
        /// <returns>The Newsletter auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterHtmlTemplate()
        {
            return await GetTemplate(new OffersEmail.DataContracts.CampaignDataContract
            {
                UnsubscribeUrl = "test/UnsibscribeUrl"
            }, "/earn/newsletter");
        }

        /// <summary>
        /// Tests the Newsletter template August 2015.
        /// </summary>
        /// <returns>The Newsletter August 2015 auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterAugust2015HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/newsletteraugust2015");
        }

        /// <summary>
        /// Tests the Newsletter template September 2015.
        /// </summary>
        /// <returns>The Newsletter September 2015 auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterSeptember2015HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/newsletterseptember2015");
        }

        /// <summary>
        /// Tests the Newsletter template November 2015.
        /// </summary>
        /// <returns>The Newsletter November 2015 auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterNovember2015HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/newsletternovember2015");
        }

        /// <summary>
        /// Tests the Newsletter template November 2015.
        /// </summary>
        /// <returns>The Newsletter November 2015 auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterNovember2015SummaryHtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/newsletternovember2015Summary");
        }

        /// <summary>
        /// Tests the Newsletter template December 2015.
        /// </summary>
        /// <returns>The Newsletter December 2015 auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterDecember2015HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/newsletterdecember2015");
        }

        /// <summary>
        /// Tests the Promo template December 2015.
        /// </summary>
        /// <returns>The Promo December 2015 auth email template</returns>
        public async Task<ContentResult> TestEarnPromoDecember2015_2x3_HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/promodecember2015_2x3");
        }

        /// <summary>
        /// Tests the Promo template December 2015.
        /// </summary>
        /// <returns>The Promo December 2015 auth email template</returns>
        public async Task<ContentResult> TestEarnPromoDecember2015_2x4_HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/promodecember2015_2x4");
        }

        /// <summary>
        /// Tests the Newsletter template Black Friday 2015.
        /// </summary>
        /// <returns>The Newsletter Black Friday 2015 auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterBlackFriday2015HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/newsletterblackfriday2015");
        }

        
        /// <summary>
        /// Tests the Newsletter template January 2016.
        /// </summary>
        /// <returns>The Newsletter January 2016 auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterJanuary2016HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/NewsletterJanuary2016");
        }

        /// <summary>
        /// Tests the Newsletter template February 2016.
        /// </summary>
        /// <returns>The Newsletter February 2016 auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterFebruary2016HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/NewsletterFebruary2016");
        }

        /// <summary>
        /// Tests the Newsletter template March 2016.
        /// </summary>
        /// <returns>The Newsletter March 2016 auth email template</returns>
        public async Task<ContentResult> TestEarnNewsletterMarch2016HtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel(), "/earn/NewsletterMarch2016");
        }

        /// <summary>
        /// Tests the CardLink auth template.
        /// </summary>
        /// <returns>The CardLink auth email template</returns>
        public async Task<ContentResult> TestEarnHtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel
            {
                UserName = "Steve",
                MerchantName = "Whole Foods",
                Percent = (float)7.00
            }, "/earn/earn");
        }

        /// <summary>
        /// Tests the CardLink auth template.
        /// </summary>
        /// <returns>The CardLink auth email template</returns>
        public async Task<ContentResult> TestEarnSmsTemplate()
        {
            return await GetTemplate(new CardLinkModel
            {
                MerchantName = "Whole Foods",
                Percent = (float)7.00
            }, "/earn/earnsms");
        }

        /// <summary>
        /// Tests the CardLink auth template.
        /// </summary>
        /// <returns>The CardLink auth email template</returns>
        public async Task<ContentResult> TestBurnHtmlTemplate()
        {
            return await GetTemplate(new CardLinkModel
            {
                UserName = "Steve",
                CreditAmount = "$3.33"
            }, "/earn/burn");
        }

        /// <summary>
        /// Tests the CardLink auth template.
        /// </summary>
        /// <returns>The CardLink auth email template</returns>
        public async Task<ContentResult> TestBurnSmsTemplate()
        {
            return await GetTemplate(new CardLinkModel
            {
                CreditAmount = "$3.33"
            }, "/earn/burnsms");
        }

        /// <summary>
        /// Tests the CardLink auth template for unauthenticated sign up scenario where the user email is unconfirmed.
        /// </summary>
        /// <returns>The CardLink auth email template for unauthenciated sign up sceanrio where the user email is unconfirmed.</returns>
        public async Task<ContentResult> TestCardLinkAuthTemplateForUnAuthUserWithUnConfirmedEmail()
        {
            return await GetCardLinkAuthOrSettleTemplate("Auth", TestData.GetCardLink("UnConfirmedEmail"));
        }

        /// <summary>
        /// Tests the CardLink auth template for unauthenticated sign up scenario where the user accont is not linked.
        /// </summary>
        /// <returns>The CardLink auth email template for unauthenticated sign up scenario where the user accont is not linked.</returns>
        public async Task<ContentResult> TestCardLinkAuthTemplateForUnAuthUserWithUnlinkedAccountEmail()
        {
            return await GetCardLinkAuthOrSettleTemplate("Auth", TestData.GetCardLink("UnLinkedEmail"));
        }

        /// <summary>
        /// Tests the CardLink auth template having the feedback link.
        /// </summary>
        /// <returns>The CardLink auth email template having the feedback link</returns>
        public async Task<ContentResult> TestCardLinkAuthTemplateWithFeedbackLink()
        {
            return await GetCardLinkAuthOrSettleTemplate("Auth", TestData.GetCardLinkWithFeedbackData());
        }

        /// <summary>
        /// Tests the CardLink auth text template.
        /// </summary>
        /// <returns>The CardLink auth text template.</returns>
        public async Task<ContentResult> TestCardLinkAuthTextTemplate()
        {
            return await GetCardLinkAuthOrSettleTextTemplate("Auth", TestData.GetCardLink());
        }

        /// <summary>
        /// Tests the CardLink auth text template.
        /// </summary>
        /// <returns>The CardLink auth text template.</returns>
        public async Task<ContentResult> TestCardLinkAuthTextTemplateForUnAuthUserWithUnConfirmedEmail()
        {
            return await GetCardLinkAuthOrSettleTextTemplate("Auth", TestData.GetCardLink("UnConfirmedEmail"));
        }

        /// <summary>
        /// Tests the CardLink auth text template.
        /// </summary>
        /// <returns>The CardLink auth text template.</returns>
        public async Task<ContentResult> TestCardLinkAuthTextTemplateForUnAuthUserWithUnlinkedAccountEmail()
        {
            return await GetCardLinkAuthOrSettleTextTemplate("Auth", TestData.GetCardLink("UnLinkedEmail"));
        }

        /// <summary>
        /// Tests the CardLink settle template.
        /// </summary>
        /// <returns>
        /// The CardLink settle email template
        /// </returns>
        public async Task<ContentResult> TestCardLinkSettleTemplate()
        {
            return await GetCardLinkAuthOrSettleTemplate("Settle", TestData.GetCardLink());
        }

        /// <summary>
        /// Tests the CardLink auth template for unauthenticated CLO signup sceanrio where the email is unconfirmed..
        /// </summary>
        /// <returns>The CardLink auth email template for unauthenticated CLO signup sceanrio where the email is unconfirmed.</returns>
        public async Task<ContentResult> TestCardLinkSettleTemplateForUnAuthUserWithUnConfirmedEmail()
        {
            return await GetCardLinkAuthOrSettleTemplate("Settle", TestData.GetCardLink("UnConfirmedEmail"));
        }

        /// <summary>
        /// Tests the CardLink auth template for unauthenticated CLO signup sceanrio where the account is not linked.
        /// </summary>
        /// <returns>The CardLink auth email template for unauthenticated CLO signup sceanrio where the account is not linked.</returns>
        public async Task<ContentResult> TestCardLinkSettleTemplateForUnAuthUserWithUnlinkedAccountEmail()
        {
            return await GetCardLinkAuthOrSettleTemplate("Settle", TestData.GetCardLink("UnLinkedEmail"));
        }

        /// <summary>
        /// Tests the CardLink settle template having the feedback link.
        /// </summary>
        /// <returns>
        /// The CardLink settle email template having the feedback link
        /// </returns>
        public async Task<ContentResult> TestCardLinkSettleTemplateWithFeedbackLink()
        {
            return await GetCardLinkAuthOrSettleTemplate("Settle", TestData.GetCardLinkWithFeedbackData());
        }

        /// <summary>
        /// Tests the CardLink settle text template.
        /// </summary>
        /// <returns>
        /// The CardLink settle text template.
        /// </returns>
        public async Task<ContentResult> TestCardLinkSettleTextTemplate()
        {
            return await GetCardLinkAuthOrSettleTextTemplate("Settle", TestData.GetCardLink());
        }

        /// <summary>
        /// Tests the CardLink auth text template for unauthenticated CLO signup sceanrio where the email is unconfirmed..
        /// </summary>
        /// <returns>The CardLink auth text template for unauthenticated CLO signup sceanrio where the email is unconfirmed.</returns>
        public async Task<ContentResult> TestCardLinkSettleTextTemplateForUnAuthUserWithUnConfirmedEmail()
        {
            return await GetCardLinkAuthOrSettleTextTemplate("Settle", TestData.GetCardLink("UnConfirmedEmail"));
        }

        /// <summary>
        /// Tests the CardLink auth text template for unauthenticated CLO signup sceanrio where the account is not linked.
        /// </summary>
        /// <returns>The CardLink auth text template for unauthenticated CLO signup sceanrio where the account is not linked.</returns>
        public async Task<ContentResult> TestCardLinkSettleTextTemplateForUnAuthUserWithUnlinkedAccountEmail()
        {
            return await GetCardLinkAuthOrSettleTextTemplate("Settle", TestData.GetCardLink("UnLinkedEmail"));
        }

        /// <summary>
        /// Tests the card link auth SMS template.
        /// </summary>
        /// <returns>The CardLink auth SMS template</returns>
        public async Task<ContentResult> TestCardLinkAuthSmsTemplate()
        {
            return await GetTemplate(TestData.GetCardLink(), "/CardLink/AuthSms/testemailcampaign/");
        }

        /// <summary>
        /// Tests the CardLink add card template.
        /// </summary>
        /// <returns>The CardLink add card email template</returns>
        public async Task<ContentResult> TestCardLinkAddCardTemplate()
        {
            return await GetTemplate(TestData.GetCardLink(), "/CardLink/AddCard/testemailcampaign/");
        }

        /// <summary>
        /// Tests the CardLink add card template.
        /// </summary>
        /// <returns>The CardLink add card email template</returns>
        public async Task<ContentResult> TestCardLinkAddCardTemplateForUnverifiedUser()
        {
            return await GetTemplate(TestData.GetCardLink("UnConfirmedEmail"), "/CardLink/AddCard/testemailcampaign/");
        }

        /// <summary>
        /// Tests the card link add card SMS template.
        /// </summary>
        /// <returns>he CardLink add card SMS template</returns>
        public async Task<ContentResult> TestCardLinkAddCardSmsTemplate()
        {
            return await GetTemplate(TestData.GetCardLink(), "/CardLink/AddCardSms/testemailcampaign/");
        }

        /// <summary>
        /// Tests the card link add card SMS template.
        /// </summary>
        /// <returns>he CardLink add card SMS template</returns>
        public async Task<ContentResult> TestCardLinkAddCardSmsTemplateForUnverifiedUser()
        {
            return await GetTemplate(TestData.GetCardLink("UnConfirmedEmail"), "/CardLink/AddCardSms/testemailcampaign/");
        }

        /// <summary>
        /// Tests the CardLink delete card template.
        /// </summary>
        /// <returns>he CardLink delete card email template</returns>
        public async Task<ContentResult> TestCardLinkDeleteCardTemplate()
        {
            return await GetTemplate(TestData.GetCardLink(), "/CardLink/DeleteCard/testemailcampaign/");
        }

        /// <summary>
        /// Tests the CardLink complete profile email template.
        /// </summary>
        /// <returns>The CardLink complete profile email template</returns>
        public async Task<ContentResult> TestCardLinkCompleteProfileTemplate()
        {
            return await GetTemplate(TestData.GetCardLink(), "/CardLink/CompleteProfile/testemailcampaign/");
        }

        /// <summary>
        /// Tests the email deal share template.
        /// </summary>
        /// <returns>The email deal share template</returns>
        public async Task<ContentResult> TestEmailDealShareTemplate()
        {
            return await GetTemplate(TestData.GetShareDealsModel(1), "/Share/Deal");
        }

        /// <summary>
        /// Tests the email deal share template.
        /// </summary>
        /// <returns>The email deal share template</returns>
        public async Task<ContentResult> TestEmailPageShareTemplate()
        {
            return await GetTemplate(TestData.GetShareDealsModel(3), "/Share/Deal");
        }

        /// <summary>
        /// Tests the reminder activate account template.
        /// </summary>
        /// <param name="id">The city name.</param>
        /// <returns>
        /// ReminderActivateAccount Template
        /// </returns>
        public async Task<ContentResult> TestReminderActivateAccountTemplate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return await GetTemplate(TestData.GetReminderActivateAccountModel(), "/Reminder/ActivateAccount/email_test/test_email");
            }

            return await GetTemplate(TestData.GetReminderActivateAccountWithBusinessesModel(id), string.Concat("/Reminder/ActivateAccountB/email_test/", id));
        }

        /// <summary>
        /// Tests DeprecateFacebook Template.
        /// </summary>
        /// <returns>
        /// DeprecateFacebook Template
        /// </returns>
        public async Task<ContentResult> TestDeprecateFacebookTemplate()
        {
            return await GetTemplate(TestData.GetReminderActivateAccountModel(), "/Reminder/DeprecateFacebook/email_test/test_email");
        }

        /// <summary>
        /// Tests TestBingOffersUpdateNotice Template.
        /// </summary>
        /// <returns>
        /// TestBingOffersUpdateNotice Template
        /// </returns>
        public async Task<ContentResult> TestBingOffersUpdateNotice()
        {
            return await GetTemplate(TestData.GetReminderActivateAccountModel(), "/Reminder/BingOffersUpdateNotice/email_test/test_email");
        }

        /// <summary>
        /// Tests TestDeprecateFacebookTemplateNew Template.
        /// </summary>
        /// <returns>
        /// TestBingOffersUpdateNotice Template
        /// </returns>
        public async Task<ContentResult> TestDeprecateFacebookTemplateNew()
        {
            return await GetTemplate(TestData.GetReminderActivateAccountModel(), "/Earn/DeprecateFacebook");
        }

        /// <summary>
        /// Tests migrate user Template.
        /// </summary>
        /// <returns>
        /// TestMigrateUserTemplate Template
        /// </returns>
        public async Task<ContentResult> TestMigrateUserTemplate()
        {
            var model = TestData.GetReminderActivateAccountModel();
            model.Content = "https://www.earnbymicrosoft.com/Migrate?ut=Bt7YQtpNxol2ld03qifcmqvrJ0izByQ9T5SCcpV1ReSTJNPbur6K6Y2L3Cxd4Vs6";
            return await GetTemplate(model, "/Earn/MigrateUser");
        }

        /// <summary>
        /// Tests the Hero Tile Preview Html template.
        /// </summary>
        /// <returns>HeroTilePreviewHtml Template</returns>
        public async Task<ContentResult> TestHeroTilePreviewHtml()
        {
            return await GetTemplate(TestDealPreview.GetCardlinkDealPreviewModel(), "/Preview/HeroTileHtml");
        }

        /// <summary>
        /// Tests the hero tile preview image.
        /// </summary>
        /// <returns>HeroTilePreviewImage Template</returns>
        public async Task<ActionResult> TestHeroTilePreviewImage()
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(GetUri("/Preview/HeroTileImage").AbsoluteUri, TestDealPreview.GetCardlinkDealPreviewModel());
                response.EnsureSuccessStatusCode();
                return File(await response.Content.ReadAsStreamAsync(), MediaTypeNames.Image.Jpeg);
            }
        }

        /// <summary>
        /// Tests the hero tile preview image upload.
        /// </summary>
        /// <returns>HeroTilePreviewImage upload status</returns>
        public async Task<ContentResult> TestHeroTilePreviewImageUpload()
        {
            return await GetTemplate(TestDealPreview.GetCardlinkDealPreviewModel(), "/Preview/UploadHeroTileImage");
        }

        /// <summary>
        /// Tests the merchant billing statement.
        /// </summary>
        /// <returns>MerchantBillingStatement html</returns>
        public async Task<ActionResult> TestMerchantBillingStatementHtml()
        {
            return await GetTemplate(TestMerchantBillingStatement.GetData(), "/Report/MerchantBillingStatement");
        }

        /// <summary>
        /// Tests the merchant billing statement.
        /// </summary>
        /// <returns>MerchantBillingStatement html</returns>
        public async Task<ActionResult> TestMerchantBillingStatementPdf()
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(GetUri("/Report/MerchantBillingStatementPdf").AbsoluteUri, TestMerchantBillingStatement.GetData());
                response.EnsureSuccessStatusCode();
                return File(await response.Content.ReadAsStreamAsync(), MediaTypeNames.Application.Pdf);
            }
        }

        /// <summary>
        /// Tests the daily deals template.
        /// </summary>
        /// <param name="id">The template identifier.</param>
        /// <returns>
        /// the daily deals email template
        /// </returns>
        public async Task<ContentResult> TestDailyDealsTemplate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return await GetTemplate(TestData.GenerateDealsJson(3, DealType.Prepaid), "/GetEmail/DailyDeals");
            }

            return await GetTemplate(TestData.GenerateDealsJson(4), id);
        }

        #endregion

        #region Private members
        /// <summary>
        /// Gets the CardLink auth or settle template.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="authJson">The json payload.</param>
        /// <returns>
        /// The CardLink auth or settle template.
        /// </returns>
        private async Task<ContentResult> GetCardLinkAuthOrSettleTemplate(string view, object authJson)
        {
            var template = view.Equals("Auth") ? "/CardLink/Auth" : "/CardLink/Settle";
            return await GetTemplate(authJson, template);
        }

        /// <summary>
        /// Gets the CardLink auth or settle text template.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="authJson">The json payload.</param>
        /// <returns>
        /// The CardLink auth or settle text template.
        /// </returns>
        private async Task<ContentResult> GetCardLinkAuthOrSettleTextTemplate(string view, object authJson)
        {
            var template = view.Equals("Auth") ? "/CardLink/Auth" : "/CardLink/Settle";
            return await GetTextTemplate(authJson, template);
        }

        /// <summary>
        /// Get email template
        /// </summary>
        /// <param name="model">json model</param>
        /// <param name="url">action url</param>
        /// <returns>email template html</returns>
        private async Task<ContentResult> GetTemplate(object model, string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(GetUri(url).AbsoluteUri, model);
                response.EnsureSuccessStatusCode();
                return new ContentResult { Content = await response.Content.ReadAsStringAsync(), ContentType = "text/html" };
            }
        }

        /// <summary>
        /// Get email template
        /// </summary>
        /// <param name="json">json string</param>
        /// <param name="url">action url</param>
        /// <returns>email template html</returns>
        private async Task<ContentResult> GetTemplate(string json, string url)
        {
            var jsonObject = JsonConvert.DeserializeObject(json);
            return await GetTemplate(jsonObject, url);
        }

        /// <summary>
        /// Get text template
        /// </summary>
        /// <param name="json">json string</param>
        /// <param name="url">action url</param>
        /// <returns>email template html</returns>
        private async Task<ContentResult> GetTextTemplate(string json, string url)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage
                {
                    RequestUri = this.GetUri(url),
                    Method = HttpMethod.Post
                })
                {
                    HttpContent httpContent = new StringContent(json);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                    request.Content = httpContent;

                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return new ContentResult { Content = await response.Content.ReadAsStringAsync(), ContentType = "text/html" };
                }
            }
        }

        /// <summary>
        /// Get text template
        /// </summary>
        /// <param name="json">json string</param>
        /// <param name="url">action url</param>
        /// <returns>email template html</returns>
        private async Task<ContentResult> GetTextTemplate(object json, string url)
        {
            return await GetTextTemplate(JsonConvert.SerializeObject(json), url);
        }

        /// <summary>
        /// Get Uri
        /// </summary>
        /// <param name="path">url path</param>
        /// <returns>Complete uri along with path</returns>
        private Uri GetUri(string path)
        {
            var requestUri = Request.Url;
            var uriBuilder = new UriBuilder(requestUri.Scheme, requestUri.Host, requestUri.Port);
            var uri = new Uri(uriBuilder.Uri, path);

            ////var newPort = requestUri.Port - 1;
            ////var url = uri.AbsoluteUri.Replace(":" + requestUri.Port, ":" + newPort);
            ////uri = new Uri(url);

            return uri;
        }
        #endregion
    }
}