/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    window,document*/
/*
    cookie helper.
*/

(function () {
    "use strict";
    window.cookie = {
        writeCookie: function (name, value, days) {
            var expires, date, hostName = window.location.hostname,
                domain = "; domain=" + hostName.substring(hostName.indexOf('.') + 1);
            if (days) {
                date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = "; expires=" + date.toGMTString();
            } else {
                expires = "";
            }
            document.cookie = name + "=" + value + expires + domain + "; path=/";
        },

        readCookie: function (name) {
            var cookie, i,
                nameEq = name + "=",
                cookies = document.cookie.split(";"),
                result = null;

            for (i = 0; i < cookies.length; i = i + 1) {
                cookie = cookies[i];
                while (cookie.charAt(0) === " ") {
                    cookie = cookie.substring(1, cookie.length);
                }
                if (cookie.indexOf(nameEq) === 0) {
                    result = cookie.substring(nameEq.length, cookie.length);
                }
            }

            return result;
        },

        eraseCookie: function (name) {
            this.writeCookie(name, "", -1);
        }
    };
}());