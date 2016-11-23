/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    $,
            configuration,window*/
(function () {
    "use strict";
    var addCard = function (serializedCardInfo, userToken) {
        return $.ajax({
            type: 'POST',
            timeout: configuration.commerceApiTimeout,
            dataType: 'json',
            url: configuration.addCardEndPoint,
            data: serializedCardInfo,
            beforeSend: function (request) {
                request.setRequestHeader(
                    "Authorization",
                    "usertoken " + userToken
                );

                request.setRequestHeader(
                    "Content-Type",
                    "application/json; charset=utf-8"
                );
            }
        });
    };

    var addCardV2 = function (cardId, userToken, flightId, referrerName) {
        var obj = {
            number: cardId,
            flight_id: flightId,
            referrer: referrerName
        };

        return $.ajax({
            type: 'POST',
            timeout: configuration.commerceApiTimeout,
            dataType: 'json',
            url: configuration.addCardEndPointV2,
            data: JSON.stringify(obj),
            beforeSend: function (request) {
                request.setRequestHeader(
                    "Authorization",
                    "usertoken " + userToken
                );

                if (flightId === "333" || flightId === "101") {
                    request.setRequestHeader("X-Flight-ID", "Earn");
                }

                request.setRequestHeader(
                    "Content-Type",
                    "application/json; charset=utf-8"
                );
            }
        });
    };

    var addCardUnauthenticated = function (cardId, email, referrer, locationContext) {

        var obj = {
            number: cardId,
            email: email,
            referrer: referrer,
            user_loc: locationContext
        };

        return $.ajax({
            type: 'POST',
            timeout: configuration.commerceApiTimeout,
            dataType: 'json',
            url: configuration.addCardUnAuthenticatedEndPoint,
            data: JSON.stringify(obj),
            beforeSend: function (request) {
                request.setRequestHeader(
                    "Content-Type",
                    "application/json; charset=utf-8"
                );
            }
        });
    };

    window.cardsControllerProxy = {
        addCard: addCard,
        addCardV2: addCardV2,
        addCardUnauthenticated: addCardUnauthenticated
    };
}());