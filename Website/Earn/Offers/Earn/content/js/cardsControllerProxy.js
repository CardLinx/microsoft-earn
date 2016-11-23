/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    $, configuration,*/

(function () {
    "use strict";
    var deleteCard = function (cardId) {
        return $.ajax({
            type: "POST",
            dataType: "json",
            timeout: configuration.commerceApiTimeout,
            url: configuration.apis.commerceServer.cards + "?cardId=" + cardId
        });
    },

        getRegisteredCards = function () {
            return $.ajax({
                type: "GET",
                dataType: "json",
                timeout: configuration.commerceApiTimeout,
                url: configuration.apis.commerceServer.cardsV2
            });
        };

    window.cardsControllerProxy = {
        deleteCard: deleteCard,
        getRegisteredCards: getRegisteredCards
    };
}());