/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    "use strict";
    var request = {
        contentType: "application/json",
        get: "GET",
        post: "POST",
        getTransactionsUrl: "/api/commerce/FetchTransactions",
        getSettlementsUrl: "/api/commerce/FetchSettlements",
        getReferralsUrl: "/api/commerce/FetchReferrals",
        issueCreditsUrl: "/api/commerce/IssueCredits",
        getEarnBurnLineItemsUrl: "/api/commerce/FetchEarnBurnLineItems",
        getEarnBurnHistoryUrl: "/api/commerce/FetchEarnBurnHistory",
    },

    getTransactions = function (requestParameters) {
        return $.Deferred(function (def) {
            $.ajax({
                url: request.getTransactionsUrl,
                type: request.get,
                contentType: request.contentType,
                data: requestParameters
            }).done(function (transactions) {
                def.resolve(transactions);
            }).fail(function (errorResponse) {
                def.reject(errorResponse);
            });
        }).promise();
    },

    getSettlements = function (requestParameters) {
        return $.Deferred(function (def) {
            $.ajax({
                url: request.getSettlementsUrl,
                type: request.get,
                contentType: request.contentType,
                data: requestParameters
            }).done(function (settlements) {
                def.resolve(settlements);
            }).fail(function (errorResponse) {
                def.reject(errorResponse);
            });
        }).promise();
    },

    getReferrals = function (requestParameters) {
        return $.Deferred(function (def) {
            $.ajax({
                url: request.getReferralsUrl,
                type: request.get,
                contentType: request.contentType,
                data: requestParameters
            }).done(function (referrals) {
                def.resolve(referrals);
            }).fail(function (errorResponse) {
                def.reject(errorResponse);
            });
        }).promise();
    },

    issueCredits = function (requestParameters) {
        return $.Deferred(function (def) {
            $.ajax({
                url: request.issueCreditsUrl,
                type: request.post,
                contentType: request.contentType,
                data: JSON.stringify(requestParameters),
            }).done(function (response) {
                def.resolve(response);
            }).fail(function (errorResponse) {
                def.reject(errorResponse);
            });
        }).promise();
    },

    getEarnBurnHistory = function (userId) {
        return $.Deferred(function (def) {
            $.ajax({
                url: request.getEarnBurnHistoryUrl,
                type: request.get,
                contentType: request.contentType,
                data: userId,
            }).done(function (data) {
                def.resolve(data);
            }).fail(function (errorResponse) {
                def.reject(errorResponse);
            });
        }).promise();
    },

    getEarnBurnLineItems = function (userId) {
        return $.Deferred(function (def) {
            $.ajax({
                url: request.getEarnBurnLineItemsUrl,
                type: request.get,
                contentType: request.contentType,
                data: userId,
            }).done(function (data) {
                def.resolve(data);
            }).fail(function (errorResponse) {
                def.reject(errorResponse);
            });
        }).promise();
    }

    window.commerceService = {
        getTransactions: getTransactions,
        getSettlements: getSettlements,
        getReferrals: getReferrals,
        issueCredits: issueCredits,
        getEarnBurnLineItems: getEarnBurnLineItems,
        getEarnBurnHistory: getEarnBurnHistory
    };
}());