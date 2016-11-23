/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global $, document, window, messaging, card, cardsControllerProxy  */
(function () {
    "use strict";
    var userToken = "";
    var parentUrl = "";
    var emptyForm = function () {
        $("#cardlink-addCard-screen-error").text("");
        $("#cardlink-cardNumber").val("");
        $("#cardlink-cardExpiryYear").prop('selectedIndex', 0);
        $("#cardlink-cardExpiryMonth").prop('selectedIndex', 0);
        $("#cardlink-cardBrand").prop('selectedIndex', 0);
    };

    var onCardSucessfullyAdded = function (cardId) {
        emptyForm();
        messaging.sendMessageToParent(parentUrl, messaging.sendMessageNames.cardAdded + cardId);
    };

    var onUserTokenExpired = function () {
        messaging.sendMessageToParent(parentUrl, messaging.sendMessageNames.tokenExpired);
    };

    var onAddCard = function () {
        $("#cardlink-addCard-screen-error").text("");
        var number = $("#cardlink-cardNumber").val();
        if (number) {
            number = number.replace(/\s+/g, '');
        }
        var creditCardInfo = card.cardInfo("1",
                number,
                $("#cardlink-cardExpiryYear :selected").val(),
                $("#cardlink-cardExpiryMonth :selected").val(),
                $("#cardlink-cardBrand :selected").val());

        if (creditCardInfo.validateCard()) {
            $.when(cardsControllerProxy.addCard(
                creditCardInfo.getSerializedCardInfo(),
                userToken
            )).done(function (data) {
                onCardSucessfullyAdded(data.new_card_id);
            }).fail(function (xhr) {
                try {
                    if ((xhr.status === 403) || (xhr.status === 500)) {
                        var responseDetails = JSON.parse(xhr.responseText);
                        $("#cardlink-addCard-screen-error")
                            .text(responseDetails.result_summary.explanation);
                    } else {
                        $("#cardlink-addCard-screen-error")
                            .text("An unknown error occured. Please try again");
                    }
                } catch (err) {
                    $("#cardlink-addCard-screen-error")
                            .text("An unknown error occured. Please try again");
                }
            });
        } else {
            $("#cardlink-addCard-screen-error")
                .text("That isn’t a valid credit card. Check the card type, card number, and expiry date and try again.");
        }
    };

    var onCancelWizard = function () {
        emptyForm();
        messaging.sendMessageToParent(parentUrl, messaging.sendMessageNames.cancelWizard);
    };

    $(document).ready(function () {
        $('.cardlink_addCard_add').on('click', onAddCard);
        $('.cardlink_addCard_Cancel').on('click', onCancelWizard);
    });

    $(window).on("hashchange", function () {
        var message = messaging.getMessageFromParent();
        if (message !== null) {
            switch (message.name) {
            case messaging.receiveMessageNames.userTokenReceived:
                userToken = message.value;
                break;

            case messaging.receiveMessageNames.parentUrlUpdate:
                parentUrl = decodeURIComponent(message.value);
                break;
            }
        }
    });
}());