/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    window,
            configuration*/

(function () {
    "use strict";

    var sendCommandsList = [configuration.commands.sendCommands.userTokenUpdated,
        configuration.commands.sendCommands.parentUriUpdated,
        configuration.commands.sendCommands.parentSubmitClicked],
        receiveCommandsList = [configuration.commands.receiveCommands.cancelButtonClicked,
            configuration.commands.receiveCommands.cardSuccessfullyAdded,
            configuration.commands.receiveCommands.userTokenExpired,
            configuration.commands.receiveCommands.goAhead],
        tryExtractSpecificCommand = function (nameValue, commandName) {
            if (nameValue === null || nameValue.length === 0) {
                return null;
            }
            var index = nameValue.indexOf(commandName);
            if (index > -1) {
                return nameValue.substring(index + commandName.length);
            }
            return null;
        },
        extractCommand = function (nameValue) {
            var i, commandValue, result = {};
            for (i = 0; i < receiveCommandsList.length; i = i + 1) {
                commandValue = tryExtractSpecificCommand(
                    nameValue,
                    receiveCommandsList[i]
                );
                if (commandValue !== null) {
                    result.name = receiveCommandsList[i];
                    result.value = commandValue;
                    return result;
                }
            }
            return null;
        },
        sendMessageToChild = function (childFrameUrl, sendCommandName, sendCommandValue) {
            var newIframeUrl = childFrameUrl +
                "#" +
                configuration.messagingProtocolPrefix +
                sendCommandName +
                sendCommandValue;
            return newIframeUrl;
        },
        getMessageFromChild = function (message) {
            if (message !== null && message.length > 0) {
                return extractCommand(message);
            }
            return null;
        };

    window.messaging = {
        sendMessageToChild: sendMessageToChild,
        getMessageFromChild: getMessageFromChild,
        receiveMessageNames: {
            cancelWizard: receiveCommandsList[0],
            cardAdded: receiveCommandsList[1],
            tokenExpired: receiveCommandsList[2],
            goAhead: receiveCommandsList[3]
        },
        sendMessageNames: {
            updateUserToken: sendCommandsList[0],
            updateParentUrl: sendCommandsList[1],
            parentSubmitClicked: sendCommandsList[2]
        }
    };
}());