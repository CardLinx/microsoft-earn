/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    window.referralCodeViewModel = function () {
        var self = this;

        self.referralCode = ko.observable(false);
        self.loaded = ko.observable();
        self.shareUrl = ko.computed(function () {
            if (self.referralCode()) {
                return configuration.queryStrings.reward.referralUrl + "/?" + configuration.queryStrings.reward.referredcode + "=" + self.referralCode();
            }
        });

        self.timerHandle;
        self.tryCount = 0;

        self.initTimer = function () {
            self.timerHandle = window.setTimeout(self.loadReferralCode, 2500);
        };

        self.loadReferralCode = function () {
            clearTimeout(self.timerHandle);
            if (!self.loaded()) {
                $.ajax({
                    url: '/Enroll/GetReferralCode',
                    type: "POST"
                }).done(function (res) {
                    self.loaded(true);
                    self.referralCode(res);
                }).error(function (e) {
                    self.loaded(false);
                    self.initTimer();
                    self.tryCount++;
                })
            }
        };

        self.startFetchingReferralCode = function () {
            self.initTimer();
        };
    };
}());