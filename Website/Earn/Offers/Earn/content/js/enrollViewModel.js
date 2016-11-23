/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    window.enrollViewModel = function (EnrollServerModel, currentStep, user_id) {

        var self = this;

        var shareConfig = {
            share_url: configuration.queryStrings.reward.referralUrl,
            image_url: 'https://az414848.vo.msecnd.net/card-linked-assets/earn-fb-referandearn-v2.1.jpg',
            title: 'Enroll for free and get $5 in bonus Earn Credits!',
            redirect_uri: "https://earn.microsoft.com/facebook",
            name: 'EARN.MICROSOFT.COM',
            description: 'Earn at thousands of participating merchants and get the technology you love. Click or tap to enroll using my personal referral link.',
            twitterTitle: 'Enroll for free and get rewarded. Use my link to get $5 in bonus Earn Credits!',
            emailBody: 'Earn at thousands of participating merchants and get the technology you love. Use my personal referral link and get $5 in bonus Earn Credits!'
        };

        EnrollServerModel = EnrollServerModel || {};
        currentStep = currentStep || 'step1';
        self.analyticsTracking = new window.bouxAnalytics({
            enabled: configuration.analytics.enableEarnAnalytics,
            flightModel: null
        });

        self.referralCodeView = new window.referralCodeViewModel();
        self.referralCodeView.shareUrl.subscribe(function (newValue) {
            shareConfig.share_url = newValue;
        });

        self.selectedEnrollTerms = ko.observable(false);
        self.settingsView = new window.settingsViewModel(self, EnrollServerModel.user_info, user_id, EnrollServerModel.cards, true, self.analyticsTracking);
        self.socialView = new window.socialPluginViewModel(self.analyticsTracking, shareConfig);

        self.currentStep = ko.observable(currentStep);

        self.enrollError = ko.computed(function () {
            var contactInfoError = self.settingsView.contactView.error();
            if (contactInfoError) {
                return contactInfoError;
            }
        });

        self.resetAllErrors = function () {
            self.settingsView.contactView.error('');
        };

        self.enrollClicked = function () {
            self.resetAllErrors();
            if (!self.settingsView.contactView.validateFields()) {
                return;
            }

            if (!self.selectedEnrollTerms()) {
                self.settingsView.contactView.error('You need to agree to terms of use');
                return;
            }

            self.settingsView.contactView.saveClicked(true, self.contactSavedSuccessfully);
            self.analyticsTracking.trackEvent('user.registered', 'registration');
        };

        self.contactSavedSuccessfully = function () {
            self.settingsView.addCardClicked();
            self.currentStep('step2');
            self.referralCodeView.startFetchingReferralCode();
            self.analyticsTracking.trackEvent('step2', 'registration');
            RioTracking.click(300385519, '11087206757330', 'USCMO_EPPEarn_Vue_Sit_StepOne_CON');
        };

        self.goToStep3Clicked = function () {
            self.currentStep('step3');
            self.analyticsTracking.trackEvent('step3', 'registration');
            RioTracking.click(300385518, '11087206757330', 'USCMO_EPPEarn_Vue_Sit_StepTwo_FNL');
        };

        self.completeClicked = function () {
            window.location.href = '/';
        };

        self.analyticsTracking.trackEvent('step1', 'registration');
        RioTracking.click(300385516, '11087206757330', 'USCMO_EPPEarn_Vue_Sit_Enroll_CON');
    };
}());