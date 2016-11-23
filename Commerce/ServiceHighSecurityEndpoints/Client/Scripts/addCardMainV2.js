/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global $, document, window, messaging, cardsControllerProxy  */
(function () {
    "use strict";
    var userToken = "",
        parentUrl = "",
        flightId = "0",
        referrer = "";

    var emptyForm = function () {
        $("#cardlink-cardNumber").val("");
        $("#cardlink-addCard-screen-error").text("");
    };

    var validateCardNumber = function (cardNumber) {
        var digit,
            number = cardNumber,
            luhnModifiedValues = [0, 2, 4, 6, 8, 1, 3, 5, 7, 9],
            checksum = 0,
            modifyDigits = false,
            count;

        if (!number) {
            return false;
        }

        for (count = number.length - 1; count >= 0; count = count - 1) {
            digit = number.charAt(count);

            if (modifyDigits === false) {
                checksum += parseInt(digit, 10);
            } else {
                checksum += luhnModifiedValues[digit];
            }

            modifyDigits = !modifyDigits;
        }

        return (checksum % 10 === 0);
    };

    var isAmexCardType = function (cardNumber) {
        if (cardNumber && cardNumber.charAt(0) === "3") {
            return true;
        }
        return false;
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

        if (validateCardNumber(number)) {
            if (flightId !== '101') {
                if (isAmexCardType(number)) {
                    $("#cardlink-addCard-screen-error").text("We don’t currently support American Express credit cards.");
                    return;
                }
            }

            $('#cardlink_addCard_add').hide();
            $('#loading-icon').css('display', 'block');
                   
            $.when(cardsControllerProxy.addCardV2(number, userToken, flightId, referrer)).done(function (data) {
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
            }).always(function () {
                $("#cardlink_addCard_add").show();
                $("#loading-icon").hide();
            });
        } else {
            $("#cardlink-addCard-screen-error").text("That isn’t a valid credit or debit card number.");
        }
    };

    var onCancelWizard = function () {
        emptyForm();
        messaging.sendMessageToParent(parentUrl, messaging.sendMessageNames.cancelWizard);
    };

    var getQueryParameterValue = function (paramName) {
        var i, queryString,
            nameValues, nameValue;
        if (!window.location.search) {
            return null;
        }

        queryString = window.location.search.substring(1);
        if (queryString && queryString.length > 0) {
            nameValues = queryString.split('&');
            if (nameValues === null || nameValues.length < 1) {
                return null;
            }

            for (i = 0; i < nameValues.length; i = i + 1) {
                nameValue = nameValues[i].split('=');
                if (decodeURIComponent(nameValue[0]) === paramName) {
                    return decodeURIComponent(nameValue[1]);
                }
            }
        }

        return null;
    };

    var parseHashChangeMessage = function () {
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
    };

    $(document).ready(function () {
       // parseHashChangeMessage();
        flightId = getQueryParameterValue('flight');
        parentUrl = getQueryParameterValue('parent_url');
        userToken = getQueryParameterValue('user_token');
        referrer = getQueryParameterValue('referrer');
        $('.cardlink_addCard_add').on('click', onAddCard);
        $('.cardlink_addCard_Cancel').on('click', onCancelWizard);
    });

    //$(window).on("hashchange", parseHashChangeMessage);
}());