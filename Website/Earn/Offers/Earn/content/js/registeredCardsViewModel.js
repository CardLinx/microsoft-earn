/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
window.registeredCardsViewModel = function (parent, RegisteredCardsServerModel) {
    var self = this;

    RegisteredCardsServerModel = RegisteredCardsServerModel || {};
  
    self.registeredCards = ko.observableArray([]);
    self.errorMessageDeletingCard = ko.observable();
    self.cardsLoaded = ko.observable(false);
    self.hasAtLeastOneRegisteredCard = ko.computed(function () {
        if (!self.registeredCards() || self.registeredCards().length < 1) {
            return false;
        }

        return true;
    });
    self.loadCards = function (callback) {
        $.when(cardsControllerProxy.getRegisteredCards())
            .done(function (result) {
                self.cardsLoaded(true);
                self.registeredCards([]);
                if (result.cards) {
                    /*jslint unparam: true*/
                    $.each(result.cards, function (index, card) {
                        self.registeredCards.push(new cardInfo(card));
                    });
                    /*jslint unparam: false*/
                }

                if (callback) {
                    callback();
                }
            });

    };
    self.deleteCard = function (card) {
        self.errorMessageDeletingCard('');
        $.when(cardsControllerProxy.deleteCard(card.id)).done(function () {
            self.registeredCards.remove(function (item) { return item.id === card.id; });
        }).fail(function (xhr) {
            if ((xhr.status === 403) || (xhr.status === 500)) {
                var responseDetails = JSON.parse(xhr.responseText);
                self.errorMessageDeletingCard(responseDetails.result_summary.explanation);
            } else {
                self.errorMessageDeletingCard("An unknown error occured");
            }
        });
    };

    function removeCard(cardId, callback, retries) {
        $.when(cardsControllerProxy.deleteCard(cardId)).done(function () {
            self.registeredCards.remove(function (item) { return item.id === cardId; });
            if (callback && !self.hasAtLeastOneRegisteredCard() && !self.errorMessageDeletingCard()) {
                callback();
            }
        }).fail(function (xhr) {
            if (!self.errorMessageDeletingCard()) {
                if (retries > 0) {
                    removeCard(cardId, callback, --retries);
                } else {
                    if ((xhr.status === 403) || (xhr.status === 500)) {
                        var responseDetails = JSON.parse(xhr.responseText);
                        self.errorMessageDeletingCard(responseDetails.result_summary.explanation);
                    } else {
                        self.errorMessageDeletingCard("An unknown error occured");
                    }
                    if (callback) {
                        callback();
                    }
                }
            }
        });
    }

    self.removeAllCards = function (callback) {
        if (login.isUserAuthenticated()) {
            if (self.hasAtLeastOneRegisteredCard()) {
                self.errorMessageDeletingCard("");
                ko.utils.arrayForEach(self.registeredCards(), function (card) {
                    removeCard(card.id, callback, configuration.maxReTries);
                });
            } else {
                if (callback) {
                    callback();
                }
            }
        }
    };

    self.init = function () {
        if (RegisteredCardsServerModel) {
            $.each(RegisteredCardsServerModel, function (index, card) {
                self.registeredCards.push(new cardInfo(card));
            });
        }
    };

    self.init();
};