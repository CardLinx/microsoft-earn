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
        findCustomersUrl: "/api/support/FindCustomers/?customerFilter="
    },

    findCustomers = function (filter) {
        return $.Deferred(function (def) {
            $.ajax({
                url: request.findCustomersUrl + filter,
                type: request.get,
                contentType: request.contentType
            }).done(function (transactions) {
                def.resolve(transactions);
            }).fail(function (errorResponse) {
                def.reject(errorResponse);
            });
        }).promise();
    };

    window.lomoUsers = {
        findCustomers: findCustomers
    };
}());