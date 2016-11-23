/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global $, document, window, messaging, cardsControllerProxy  */
(function () {
    "use strict";
    var parentUrl = "",
        uuid = "",
        referrer = "",
        flightId = "",
        location = "",
        emptyForm = function () {
            $("#card").val("");
            $("#email").val("");
            $(".error").hide();
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
            messaging.sendMessageToParent(parentUrl, messaging.sendMessageNames.cardAdded + cardId);
            emptyForm();
        },

        validateEmail = function (email) {
            return (email && /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email)); //ignore jslint
        },

        onAddCard = function () {
            $(".error").hide();
            $("#card").removeClass("error-input");
            $("#email").removeClass("error-input");
            $("#email-label").removeClass("error-text");
            $("#card-label").removeClass("error-text");
            $("#terms-container").removeClass("error-text");

            var email = $("#email").val();
            if (email) {
                email = email.trim();
            }
            if (!validateEmail(email)) {
                $("#email-invalid-error").show();
                $("#email").addClass("error-input");
                $("#email-label").addClass("error-text");
                return;
            }

            var number = $("#card").val();
            if (number) {
                number = number.replace(/\s+/g, '');
            }
            if (!validateCardNumber(number)) {
                $("#invalidCard-error").show();
                $("#card").addClass("error-input");
                $("#card-label").addClass("error-text");
                return;
            }
            if (flightId !== '101') {
                if (isAmexCardType(number)) {
                    $("#americanExpress-error").show();
                    $("#card").addClass("error-input");
                    $("#card-label").addClass("error-text");
                    return;
                }
            }

            if (!$('#terms').is(':checked')) {
                $("#terms-not-accepted-error").show();
                $("#terms-container").addClass("error-text");
                return;
            }

            $("#submit").hide();
            $("#loading-icon").css('display', 'inline-block');
            //end testing

            $.when(cardsControllerProxy.addCardUnauthenticated(number, email, referrer, location)).done(function (data) {
                onCardSucessfullyAdded(data.new_card_id);
            }).fail(function (xhr) {
                try {
                    if ((xhr.status === 403) || (xhr.status === 500)) {
                        var responseDetails = JSON.parse(xhr.responseText);
                        var resCode = responseDetails.result_summary.result_code;
                        if (resCode === 'ParameterCannotBeNull') {
                            $("#explanation-error").text('Please enter a valid email address and card number.');
                            $("#explanation-error").show();
                        }
                        else if (resCode === 'InvalidParameter') {
                            $("#explanation-error").text('Please enter a valid email address.');
                            $("#explanation-error").show();
                        }
                        else if (resCode === 'InvalidCard') {
                            $("#explanation-error").text('Please enter a valid credit or debit card number.');
                            $("#explanation-error").show();
                        }
                        else if (resCode === 'UserAlreadyExists') {
                            $("#explanation-error").text('This email address is already associated with a Bing Offers account.');
                            $("#explanation-error").show();
                        }
                        else if (resCode === 'UnauthenticatedUserAlreadyExists') {
                            $("#explanation-error").text('This email address has already been used to register a card.');
                            $("#explanation-error").show();
                        }
                        else {
                            $("#unknown-error").show();
                        }
                    } else {
                        $("#unknown-error").show();
                    }
                } catch (err) {
                    $("#unknown-error").show();
                }
            }).always(function () {
                $("#submit").show();
                $("#loading-icon").hide();
            });
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
        parentUrl = getQueryParameterValue('parent_url');
        referrer = getQueryParameterValue('referrer');
        flightId = getQueryParameterValue('flight');
        if (flightId === "5") {
            $('#join').text('Join Card-Linked');
        }
        location = getQueryParameterValue('location');
        $('#submit').on('click', onAddCard);
    });

}());