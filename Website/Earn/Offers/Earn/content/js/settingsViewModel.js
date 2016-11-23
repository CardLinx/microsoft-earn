/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    window.settingsViewModel = function (parent, UserInfoServerModel, user_id, RegisteredCardsServerModel, isEnroll, analyticsTracker) {
        var self = this;

        window.flightId = "333"; // earn
        window.referrer = "";
        self.contactView = new window.contactInfoViewModel(self, UserInfoServerModel, isEnroll);
        self.registeredCardsView = new window.registeredCardsViewModel(self, RegisteredCardsServerModel);
        self.rewardsView = new window.referralsViewModel(self, user_id);

        self.addCardFrameUrl = ko.observable();
        self.showAddCardIframe = ko.observable(false);

        self.userToken = ko.observable();
        self.userToken.subscribe(function () {
            if (self.userToken()) {
                self.addCardFrameUrl(configuration.apis.secureServer.addCardViewEndpointV3
                    + "?user_token=" + encodeURIComponent(self.userToken())
                    + "&flight=" + window.flightId
                    + "&referrer=" + window.referrer
                    + "&parent_url=" + encodeURIComponent(window.location.href));
            }
        });

        self.addCardClicked = function () {
            if (window.location.search) {
                var enableAmex = helper.getQueryParameterValue('tryamex');
                if (enableAmex && enableAmex === '1') {
                    window.flightId = '101';
                }
            }
            self.fetchUserToken(function () {
                self.showAddCardIframe(true);
            });
        };

        self.cancelAddCard = function () {
            self.showAddCardIframe(false);
        };

        self.fetchUserToken = function (callback) {
            $.when(userServices.getUserToken(self.rewardsView.referredCode, window.referrer)).done(function (data) {
                self.userToken(data.token);
                if (callback) {
                    callback();
                }
            });
        };

        self.cardSucessfullyAdded = function () {
            self.registeredCardsView.loadCards();
            self.showAddCardIframe(false);

            if (analyticsTracker) {
                analyticsTracker.trackEvent('card.added', isEnroll ? 'registration' : 'settings');
            }
        };

        self.onCrossDomainMessageReceived = function (message) {
            var result = messaging.getMessageFromChild(message);
            if (result !== null) {
                /*ignore jslint start*/
                switch (result.name) {
                    case messaging.receiveMessageNames.cancelWizard:
                        self.cancelAddCard();
                        break;
                    case messaging.receiveMessageNames.cardAdded:
                        self.cardSucessfullyAdded(result.value);
                        break;
                    case messaging.receiveMessageNames.tokenExpired:
                        self.fetchUserToken();
                        break;
                    case messaging.receiveMessageNames.goAhead:
                        self.cardsScreenSubmitClicked(true);
                        break;
                }
                /*ignore jslint end*/
            }
        };

        function processUrlHash() {
            var i, token,
                tokens = window.location.hash.substring(1).split("&");
            if (tokens.length > 0) {
                for (i = 0; i < tokens.length; i += 1) {
                    if (tokens[i]) {
                        token = tokens[i].toLowerCase();
                        if (token.beginsWith(configuration.messagingProtocolPrefix)) {
                            self.onCrossDomainMessageReceived(token);
                        }
                    }
                }

                window.location.hash = "";
            }
        }

        function initModel() {
            $(window).on("hashchange", processUrlHash);
        }

        initModel();
    };
}());