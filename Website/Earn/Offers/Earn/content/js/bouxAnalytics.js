/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    "use strict";

    window.bouxAnalytics = function (config) {
        var self = this,
            browserId,
            sessionId,
            flightId,
            newUser = false,
            campaignName,
            campaignSource,
            campaignReferrer,
            /*ignore jslint start*/
            generateGuid = function () {
                return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
                    var r = Math.random() * 16 | 0, v = c === "x" ? r : (r & 0x3 | 0x8);
                    return v.toString(16);
                });
            },
            getQueryParameterValue = function (querystring, paramName, dropTrailingSlash) {
                if (querystring) {
                    var i, nameValues, nameValue, str;
                    nameValues = querystring.split("&");
                    if (nameValues.length > 0) {
                        for (i = 0; i < nameValues.length; i = i + 1) {
                            nameValue = nameValues[i].split("=");
                            if (nameValue[0].toLowerCase() === paramName) {
                                str = nameValue[1];
                                if (dropTrailingSlash && str && str.length > 0 && str[str.length - 1] === '/') {
                                    str = str.substring(0, str.length - 1);
                                }

                                return encodeURIComponent(str);
                            }
                        }
                    }
                }
                return "";
            },
            init = function () {
                browserId = cookie.readCookie(configuration.cookies.bouxAnalytics.browser_id.name);
                var new_browser = false, new_session = false;
                if (!browserId) {
                    browserId = generateGuid();
                    cookie.writeCookie(configuration.cookies.bouxAnalytics.browser_id.name, browserId, configuration.cookies.bouxAnalytics.browser_id.expiration);
                    new_browser = true;
                    newUser = true;
                    cookie.writeCookie(configuration.cookies.bouxAnalytics.newuser.name, newUser, configuration.cookies.bouxAnalytics.newuser.expiration);
                }

                sessionId = cookie.readCookie(configuration.cookies.bouxAnalytics.session_id.name);
                if (!sessionId) {
                    sessionId = generateGuid();
                    cookie.writeCookie(configuration.cookies.bouxAnalytics.session_id.name, sessionId, configuration.cookies.bouxAnalytics.session_id.expiration);
                    new_session = true;
                }

                var result = cookie.readCookie(configuration.cookies.bouxAnalytics.newuser.name);
                if (result) {
                    newUser = true;
                }

                if (window.location.search) {
                    var querystring = window.location.search.substring(1);
                    campaignReferrer = getQueryParameterValue(querystring, configuration.queryStrings.analytics.campaignReferrer, true);
                    if (campaignReferrer && campaignReferrer.length > 0) {
                        cookie.writeCookie(configuration.cookies.campaignReferrer.name, campaignReferrer, configuration.cookies.campaignReferrer.expiration);
                    }
                }

                if (!campaignReferrer) {
                    campaignReferrer = cookie.readCookie(configuration.cookies.campaignReferrer.name) || "";
                }

                if (config.flightModel && config.flightModel.flightingEnabled) {
                    flightId = config.flightModel.flightId;
                }

                if (new_session) {
                    self.trackEvent('session.new', 'user');
                }

                if (new_browser) {
                    self.trackEvent('browser.new', 'user');
                }

                self.trackEvent('page.load', 'page');
            },
            postData = function (data) {
                if (data) {
                    $.ajax({
                        url: configuration.apis.bouxAnalytics,
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(data)
                    }).done(function () {
                    }).fail(function () {
                    });
                }
            },
            postEvent = function (eventId, eventType, eventInfo) {
                var analyticsRecord = {
                    bid: browserId,
                    sid: sessionId,
                    ptitle: document.title,
                    purl: window.location.href,
                    eid: eventId,
                    etype: eventType,
                    einfo: eventInfo,
                    flid: flightId,
                    newUser: newUser,
                    cmp_source: campaignSource,
                    cmp_name: campaignName,
                    cmp_ref: campaignReferrer
                };

                postData(analyticsRecord);
            };

        self.trackEvent = function (eventId, eventType, eventInfo) {
            if (config.enabled) {
                postEvent(eventId, eventType, eventInfo);
            }
        };

        init();
    };
}());