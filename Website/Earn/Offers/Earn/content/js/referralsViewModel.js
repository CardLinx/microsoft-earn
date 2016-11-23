/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    window.referralsViewModel = function (parent, userId, referral_code, referral_code_reports) {
        var self = this;

        self.referralCode = ko.observable(referral_code);
        self.linkClickedCount = ko.observable(0);
        self.cardlinkSignups = ko.observable(0);
        self.countOfUsersWhoRedeemed = ko.observable(0);
        self.rewardPoints = ko.observable(0);
        self.referredCode;
        self.uuid;

        self.shareUrl = ko.computed(function () {
            if (self.referralCode()) {
                return configuration.queryStrings.reward.referralUrl +  "/?" + configuration.queryStrings.reward.referredcode + "=" + self.referralCode();
            }
        });

        var shareConfig = {
            share_url: self.shareUrl(),
            image_url: 'https://az414848.vo.msecnd.net/card-linked-assets/earn_fb_referandearn.png',
            title: 'Enroll for free and get $5 in bonus Earn Credits!',
            redirect_uri: "https://earn.microsoft.com/",
            name: 'EARN.MICROSOFT.COM',
            description: 'Earn at thousands of participating merchants and get the technology you love. Click or tap to enroll using my personal referral link.',
            twitterTitle: 'Enroll for free & get rewarded. Use my link to get $5 in bonus Earn Credits!',
            emailBody: 'Earn at thousands of participating merchants and get the technology you love. Use my personal referral link and get $5 in bonus Earn Credits!'
        };

        self.socialView = new window.socialPluginViewModel(self.analyticsTracking, shareConfig);

        self.selectShareUrl = function(data, event) {
            event.currentTarget.select();
        };

        self.showCopyButton = ko.computed(function () {
            if (self.shareUrl() && self.shareUrl().length > 0 && window.clipboardData) {
                return true;
                }

            return false;
        });

        self.copyShareUrl = function () {
            window.clipboardData.setData("Text", self.shareUrl());
        }

        self.trackReferralLinkClicked = function () {
            if (self.referredCode) {
                var uniqueId = self.uuid || helper.generateGuid(),
                    obj = {
                        referral_type_code: self.referredCode,
                        referral_event: 0,
                        user_id: uniqueId
                    };

                $.ajax({
                    type: "POST",
                    dataType: "json",
                    contentType: 'application/json; charset=utf-8',
                    timeout: configuration.commerceApiTimeout,
                    url: configuration.apis.commerceServer.referralReportingEndpoint,
                    data: JSON.stringify(obj)
                });
            }
        };

        var init = function () {
            if (referral_code_reports && referral_code_reports.length > 0) {
                var friendsEnrolledButNotYetQualified = 0;
                var friendsEnrolledAndQualified = 0;
                var referralEventsPerCode = referral_code_reports[0];
                if (referralEventsPerCode.referral_event_counts && referralEventsPerCode.referral_event_counts.length > 0) {
                    $.each(referralEventsPerCode.referral_event_counts,
                        function (index, actualReferalEvent) {
                            if (actualReferalEvent.referral_event_id == 1)
                            {
                                if (!actualReferalEvent.reward_payout_status_id || actualReferalEvent.reward_payout_status_id == 0 || actualReferalEvent.reward_payout_status_id == 1)
                                {
                                    friendsEnrolledButNotYetQualified += (actualReferalEvent.count || 0);
                                }
                                else if (actualReferalEvent.reward_payout_status_id == 2)
                                {
                                    friendsEnrolledAndQualified += (actualReferalEvent.count || 0);
                                }
                            }
                        });

                    self.cardlinkSignups(friendsEnrolledButNotYetQualified);
                    self.countOfUsersWhoRedeemed(friendsEnrolledAndQualified);
                }
            }

            if (!self.uuid) {
                self.uuid = cookie.readCookie(configuration.cookies.reward.uuid.name) || "";
                if (!self.uuid) {
                    self.uuid = helper.generateGuid();
                    cookie.writeCookie(configuration.cookies.reward.uuid.name, self.uuid, configuration.cookies.reward.uuid.expiration);
                }
            }

            if (window.location.search) {
                self.referredCode = helper.getQueryParameterValue(configuration.queryStrings.reward.referredcode);
                if (self.referredCode) {
                    cookie.writeCookie(configuration.cookies.reward.referredcode.name, self.referredCode);
                    //self.trackReferralLinkClicked();
                }
            }

            // If not present then read from session cookie.
            if (!self.referredCode) {
                self.referredCode = cookie.readCookie(configuration.cookies.reward.referredcode.name) || "";
            }
        };

        init();
    }
}());