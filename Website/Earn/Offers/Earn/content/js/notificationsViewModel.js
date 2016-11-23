/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    window.notificationsViewModel = function (parent, isAddCardNotificationDisabled) {
        var self = this;

        self.showAddCardNotification = ko.observable(false);
        self.showGiftCardsIntro = ko.observable(false);

        self.checkUserRegisteredCards = function () {
            if (!parent.settingsView.registeredCardsView.hasAtLeastOneRegisteredCard()) {
                self.showNotificationIfApplicable();
            }
        }

        self.showNotificationIfApplicable = function () {
            var isRemindLaterCookieExists = cookie.readCookie(configuration.cookies.remindLaterNoCardsNotification.name);
            if (!isRemindLaterCookieExists) {
                setTimeout(function () { self.showAddCardNotification(true); }, configuration.popupNotifications.showupDelay);
            }
        };

        self.closeClicked = function () {
            cookie.writeCookie(configuration.cookies.remindLaterNoCardsNotification.name, true, configuration.cookies.remindLaterNoCardsNotification.expiration);
            self.showAddCardNotification(false);
        };

        self.init = function () {
            // checking registered user cards
            if (!parent.settingsView.registeredCardsView.cardsLoaded()) {
                parent.settingsView.registeredCardsView.loadCards(self.checkUserRegisteredCards);
            }
            else {
                self.checkUserRegisteredCards();
            }

            self.showGiftCardsIntro(parent.page != 'GiftCards');
        };

        if (!isAddCardNotificationDisabled) {
            self.init();
        }
    };
}());