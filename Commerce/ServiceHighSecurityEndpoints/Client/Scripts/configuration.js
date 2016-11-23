/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    window*/

(function () {
    "use strict";
    var messagingProtocolPrefix = "message:",
        securedCommerceCageServerHost = window.location.hostname.indexOf("commerce-secure-int") >= 0 ? "https://TODO_INSERT_YOUR_SECURE_INT_DOMAIN_HERE" : "https://TODO_INSERT_YOUR_SECURE_DOMAIN_HERE", //ignore jslint
        addCardEndPoint = securedCommerceCageServerHost + "/api/commerce/cards",
        addCardEndPointV2 = securedCommerceCageServerHost + "/api/commerce/v2/cards",
        addCardUnAuthenticatedEndPoint = securedCommerceCageServerHost + "/api/commerce/v2/cards/fast",
        commands = {
            receiveCommands: {
                userTokenUpdated: "user_token",
                parentUriUpdated: "parent_uri",
                parentSubmitClicked: "submit_clicked"
            },
            sendCommands: {
                cancelButtonClicked: "cancel_button_click",
                cardSuccessfullyAdded: "card_added",
                userTokenExpired: "user_token_expired",
                goahead: "goahead"
            }
        },
        commerceApiTimeout = 45000;

    window.configuration = {
        commands: commands,
        messagingProtocolPrefix: messagingProtocolPrefix,
        addCardEndPoint: addCardEndPoint,
        commerceApiTimeout: commerceApiTimeout,
        addCardEndPointV2: addCardEndPointV2,
        addCardUnAuthenticatedEndPoint: addCardUnAuthenticatedEndPoint
    };
}());