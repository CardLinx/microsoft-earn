/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global jQuery,$,Modernizr,window,navigator,document */

(function () {
    "use strict";

    function displayFriendlyPhoneNumber(source, locale) {
        /*ignore jslint start*/
        var stripped = source.replace(/([^\\d])/g, function () { return ""; }),
            formatted = "";
        switch (locale) {
            case "us":
                if (stripped.length > 11 || stripped.length < 10) {
                    return source;
                }
                formatted = "(" + stripped.substr(stripped.length - 10, 3) + ") " + stripped.substr(stripped.length - 7, 3) + "-" + stripped.substr(stripped.length - 4, 4);
                if (stripped.length === 11) {
                    formatted = "+" + stripped.substr(0, 1) + " " + formatted;
                }
                break;
            default:
                // We will return the unaltered string for any localities that we do not support.
                formatted = source;
        }
        /*ignore jslint end*/
        return formatted;
    }

    function displayFriendlyDate(dateString) {
        var date, day, month, year;
        if (dateString) {
            date = new Date(dateString);
            day = date.getUTCDate();
            month = date.getUTCMonth() + 1;
            year = date.getUTCFullYear();

            if (day < 10) {
                day = "0" + day;
            }
            if (month < 10) {
                month = "0" + month;
            }

            return month + "/" + day + "/" + year.toString().substring(2);
        }
        return "";
    }

    function isLowResolutionMobile() {
        return Modernizr.mq("screen and (max-width:260px)");
    }

    function isMobileView() {
        return Modernizr.mq("screen and (max-width: 709px)");
    }

    function isTabletView() {
        return Modernizr.mq("screen and (max-width: 989px)") &&
            !isMobileView();
    }

    function isSmallDesktopView() {
        return Modernizr.mq("screen and (max-width: 1449px)") &&
            !isTabletView() &&
            !isMobileView();
    }

    function allowNumeric(event) {
        var code = event.keyCode || event.which;
        // Allow: backspace, delete, tab, escape
        if (code === 46 || code === 8 || code === 9 || code === 27 ||
            // Allow: Ctrl+A
                (code === 65 && event.ctrlKey === true) ||
            // Allow: home, end, left, right
                (code >= 35 && code <= 40)) {
            return;
        }
        if (event.shiftKey ||
                (code < 48 || code > 57)) {
            event.preventDefault();
        }
    }

    function getQueryParameterValue(paramName) {
        var i, queryString,
            nameValues, nameValue;
        if (window.location.search) {
            queryString = window.location.search.substring(1);
            if (queryString && queryString.length > 0) {
                nameValues = queryString.split("&");
                if (nameValues.length > 0) {
                    for (i = 0; i < nameValues.length; i = i + 1) {
                        nameValue = nameValues[i].split("=");
                        if (nameValue.length > 1) {
                            if (nameValue[0].toLowerCase() === paramName) {
                                return decodeURIComponent(nameValue[1]);
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    function removeQueryParam(paramName) {
        if (getQueryParameterValue(paramName) === null) {
            return;
        }
        window.location.search = "";
    }

    function injectStyleSheetForSprite() {

        if (!window.helper.isLowResolutionMobile()) {
            return;
        }
        var cssId = 'low-resolotion-sprite-definition';
        if (!document.getElementById(cssId)) {
            var head = document.getElementsByTagName('head')[0];
            var link = document.createElement('link');
            link.id = cssId;
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.href = '/offers/content/css/sprite-definition';
            link.media = 'all';
            head.appendChild(link);
        }
    }

    jQuery.fn.center = function () {
        var o = this,
            centerPos = function () {
                $(o).css({
                    position: 'absolute',
                    left: ($(window).width() - $(o).outerWidth()) / 2,
                    top: ($(window).height() - $(o).outerHeight()) / 2
                });
            };

        $(window).bind('load resize scroll', function () {
            $(o).each(function () {
                centerPos();
            });
        });

        centerPos();
    };

    $.fn.hasScrollBar = function () {
        return this.get(0).scrollHeight > this.height();
    };

    String.prototype.beginsWith = function (substring) {
        return this.indexOf(substring) === 0;
    };

    String.prototype.endsWith = function (substring) {
        var neglen = this.length - substring.length;
        return this.indexOf(substring, neglen) === neglen;
    };

    if (!Array.prototype.filter) {
        Array.prototype.filter = function (fun) {

            if (this === void 0 || this === null) {
                throw new TypeError();
            }

            var t = Object(this);
            var len = t.length >>> 0;
            if (typeof fun !== "function") {
                throw new TypeError();
            }

            var res = [];
            var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
            for (var i = 0; i < len; i++) {
                if (i in t) {
                    var val = t[i];
                    if (fun.call(thisArg, val, i, t)) {
                        res.push(val);
                    }
                }
            }

            return res;
        };
    }

    var detectBrowserVersion = function () {
        var ua = navigator.userAgent,
            tem,
            M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*([\d\.]+)/i) || [];
        if (/trident/i.test(M[1])) {
            tem = /\brv[ :]+(\d+(\.\d+)?)/g.exec(ua) || [];
            return 'IE ' + (tem[1] || '');
        }
        M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
        if ((tem = ua.match(/version\/([\.\d]+)/i)) !== null) { M[2] = tem[1]; }
        return M.join(' ');
    };

    var detectedBrowserVersion = detectBrowserVersion();

    var setupAuth = function () {
        var authVal = cookie.readCookie('hmcpauth');
        if (authVal) {
            var authToken = decodeURIComponent(authVal);
            $.ajaxSetup({
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("X-FD-BingIDToken", authToken);
                    xhr.setRequestHeader("X-Flight-ID", "Earn");
                }
            });
        } else {
            $.ajaxSetup({
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("X-Flight-ID", "Earn");
                }
            });
        }
    };

    var generateGuid = function () {
        return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == "x" ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    };

    window.helper = {
        allowNumeric: allowNumeric,
        isLowResolutionMobile: isLowResolutionMobile,
        isMobileView: isMobileView,
        isTabletView: isTabletView,
        isSmallDesktopView: isSmallDesktopView,
        injectStyleSheetForSprite: injectStyleSheetForSprite,
        getQueryParameterValue: getQueryParameterValue,
        removeQueryParam: removeQueryParam,
        displayFriendlyPhoneNumber: displayFriendlyPhoneNumber,
        displayFriendlyDate: displayFriendlyDate,
        browserVersion: detectedBrowserVersion,
        generateGuid: generateGuid,
        setupAuth: setupAuth
    };
}());
