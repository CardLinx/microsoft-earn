/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    window,
            configuration*/

(function () {
    "use strict";

    //#region private members
    var receiveCommandsList = [configuration.commands.receiveCommands.userTokenUpdated, configuration.commands.receiveCommands.parentUriUpdated, configuration.commands.receiveCommands.parentSubmitClicked],
        sendCommandList = [configuration.commands.sendCommands.cancelButtonClicked,
            configuration.commands.sendCommands.cardSuccessfullyAdded,
            configuration.commands.sendCommands.userTokenExpired,
            configuration.commands.sendCommands.goahead],
        messageProtocolName = configuration.messagingProtocolPrefix,
        getNewUrl = function (oldUrl, sendCommand) {
            var hashIndex = oldUrl.indexOf("#"),
                prevHash = "",
                newHash = "",
                prevBaseUrl = oldUrl;

            if (hashIndex > 0) {
                prevHash = oldUrl.substring(hashIndex + 1);
                prevBaseUrl = oldUrl.substr(0, hashIndex);
            }

            if (prevHash.length > 0) {
                newHash = prevHash + '&' + messageProtocolName + sendCommand;
            } else {
                newHash = messageProtocolName + sendCommand;
            }

            return prevBaseUrl + '#' + newHash;
        },
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
                commandValue = tryExtractSpecificCommand(nameValue, receiveCommandsList[i]);
                if (commandValue !== null) {
                    result = {};
                    result.name = receiveCommandsList[i];
                    result.value = commandValue;
                    return result;
                }
            }
            return null;
        };
    //#endregion private members

    //#region public members
    var sendMessageToParent = function (parentUrl, sendCommand) {
        var newUrl = getNewUrl(parentUrl, sendCommand);
        window.top.location.href = newUrl;
    },
        getMessageFromParent = function () {
            var messageProtocolName = 'message:',
                hash = window.location.hash.substring(1);
            if (hash && hash.length > 0) {
                var index = hash.indexOf(messageProtocolName);
                if (index > -1) {
                    var commandNameValue = hash.substring(index + messageProtocolName.length);
                    return extractCommand(commandNameValue);
                }
            }
            return null;
        };
    //#endregion private members

    window.messaging = {
        sendMessageToParent: sendMessageToParent,
        getMessageFromParent: getMessageFromParent,
        sendMessageNames: {
            cancelWizard: sendCommandList[0],
            cardAdded: sendCommandList[1],
            tokenExpired: sendCommandList[2],
            goAhead: sendCommandList[3]
        },
        receiveMessageNames: {
            userTokenReceived: receiveCommandsList[0],
            parentUrlUpdate: receiveCommandsList[1],
            parentSubmitClicked: receiveCommandsList[2]
        }
    };
}());