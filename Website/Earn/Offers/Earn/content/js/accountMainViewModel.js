/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    window.accountMainViewModel = function (AccountsPageServerModel) {

        var self = this;
        AccountsPageServerModel = AccountsPageServerModel || {};
        self.analyticsTracking = new window.bouxAnalytics({
            enabled: configuration.analytics.enableEarnAnalytics,
            flightModel: null
        });
        self.settingsView = new window.settingsViewModel(self, AccountsPageServerModel.user_info, AccountsPageServerModel.user_id, AccountsPageServerModel.cards, false, self.analyticsTracking);
        self.supportView = new window.supportViewModel(self, AccountsPageServerModel.user_info, self.analyticsTracking);
        self.page = AccountsPageServerModel.page;

        self.localDealsView = new window.localDealsViewModel(self, AccountsPageServerModel.local_deals, AccountsPageServerModel.top_deals, AccountsPageServerModel.selected_state, AccountsPageServerModel.sort_order, AccountsPageServerModel.sort_by, self.analyticsTracking);

        self.notificationsView = new window.notificationsViewModel(self, AccountsPageServerModel.page === 'settings');
        self.referralsView = new window.referralsViewModel(self, AccountsPageServerModel.user_id, AccountsPageServerModel.referral_code, AccountsPageServerModel.referral_code_reports);
        self.transactionHistoryView = new window.transactionHistoryViewModel(AccountsPageServerModel.page === 'history');
        self.seeMicrosoftStoresClicked = function () {
            $('html, body').animate({ scrollTop: $("#storelocations").offset().top - 20 }, 1000);
        };

        self.shopOnlineClicked = function () {
            window.open("http://www.microsoftstore.com");
        };
    };
}());