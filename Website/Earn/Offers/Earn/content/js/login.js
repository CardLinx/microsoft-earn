/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    $,window,analytics,analyticsEvent,
            helper,cookie, configuration*/
/*
    login logout redirects.
*/
/*jslint maxlen: 250*/
(function () {
    "use strict";
    var isUserAuthenticated = function () {
        return true;
       // return window.session && window.session.isAuthenticated;
    };

    window.login = {
        isUserAuthenticated: isUserAuthenticated
    };
}());