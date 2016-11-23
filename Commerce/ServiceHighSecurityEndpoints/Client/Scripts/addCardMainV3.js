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
        referrer = "",
        emptyForm = function () {
            $("#card-number").val("");
            $("#error").text("");

            //if (flightId === "5") {
            //    $('#name-on-card').val("")
            //    $('#month').val("none");
            //    $('#year').val("none");
            //}
        },
        validateCardNumber = function (cardNumber) {
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
        },

        isAmexCardType = function (cardNumber) {
            if (cardNumber && cardNumber.charAt(0) === "3") {
                return true;
            }
            return false;
        },

        onCardSucessfullyAdded = function (cardId) {
            emptyForm();
            messaging.sendMessageToParent(parentUrl, messaging.sendMessageNames.cardAdded + cardId);
        },

        sendGoAheadMessageToParent = function () {
             messaging.sendMessageToParent(parentUrl, messaging.sendMessageNames.goAhead);
         },

        onAddCard = function (shouldSendGoAheadMessageToParent) {
            var number = $("#card-number").val();
            if (number) {
                number = number.replace(/\s+/g, '');
            } else {
                if (shouldSendGoAheadMessageToParent) {
                    emptyForm();
                    sendGoAheadMessageToParent();
                    return;
                }
            }

            $(".error").hide();
            $("#card-number").removeClass("error-input");

            //if (flightId === '5') {
            //    $("#name-on-card").removeClass("error-input");
            //    $("#month").removeClass("error-input");
            //    $("#year").removeClass("error-input");
            //}

            if (validateCardNumber(number)) {
                if (flightId !== '101') {
                    if (isAmexCardType(number)) {
                        $("#americanExpress-error").show();
                        $("#card-number").addClass("error-input");
                        return;
                    }
                }

                //if (flightId === '5') {
                //    var name = $('#name-on-card').val();
                //    if (!name) {
                //        $("#invalid-name").show();
                //        $("#name-on-card").addClass("error-input");
                //        return;
                //    }

                //    var selectedMonth = $('#month :selected').val();
                //    if (selectedMonth === 'none') {
                //        $("#invalid-month").show();
                //        $("#month").addClass("error-input");
                //        return;
                //    }

                //    var selectedYear = $('#year :selected').val();
                //    if (selectedYear === 'none') {
                //        $("#invalid-year").show();
                //        $("#year").addClass("error-input");
                //        return;
                //    }
                //}

                $("#add-button").hide();
                $("#loading-icon").css('display', 'inline-block');

                $.when(cardsControllerProxy.addCardV2(number, userToken, flightId, referrer)).done(function (data) {
                    if (!shouldSendGoAheadMessageToParent) {
                        onCardSucessfullyAdded(data.new_card_id);
                    } else {
                        sendGoAheadMessageToParent();
                    }
                }).fail(function (xhr) {
                    try {
                        if ((xhr.status === 403) || (xhr.status === 500)) {
                            var responseDetails = JSON.parse(xhr.responseText);
                            $("#explanation-error").text(responseDetails.result_summary.explanation);
                            $("#explanation-error").show();
                        } else {
                            $("#unknown-error").show();
                        }
                    } catch (err) {
                        $("#unknown-error").show();
                    }
                }).always(function () {
                    $("#add-button").show();
                    $("#loading-icon").hide();
                });
            } else {
                $("#invalidCard-error").show();
                $("#card-number").addClass("error-input");
            }
        },

        onCancelWizard = function () {
            emptyForm();
            messaging.sendMessageToParent(parentUrl, messaging.sendMessageNames.cancelWizard);
        },

        getQueryParameterValue = function (paramName) {
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

    $(document).ready(function () {
        userToken = getQueryParameterValue('user_token');
        parentUrl = getQueryParameterValue('parent_url');
        flightId = getQueryParameterValue('flight');
        referrer = getQueryParameterValue('referrer');
        if (flightId === '101') {
            $('#card-number').attr('placeholder', 'Enter a Visa, Amex or MasterCard number');
        } 
        $('#add-button').on('click', function () { onAddCard(false); });

        //if (flightId === '5') {
        //    $('#fullcreditcardflight').show();
        //}
    });

    $(window).on("hashchange", function () {
        var message = messaging.getMessageFromParent();
        if (message !== null) {
            switch (message.name) {
                case messaging.receiveMessageNames.parentSubmitClicked:
                    onAddCard(true);
                    break;
            }
        }
    });

}());