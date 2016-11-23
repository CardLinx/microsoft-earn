/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global $,navigator,window,document*/
/*
    logger services.
*/
(function () {
    window.logger = {
        logError: function (details) {
            "use strict";
            if (details) {
                $.ajax({
                    type: "POST",
                    url: "/api/client/log",
                    data: JSON.stringify({
                        context: navigator.userAgent,
                        details: details
                    }),
                    contentType: "application/json; charset=utf-8"
                });
            }
        }
    };
    /*jslint unparam: true*/
    $(document).ajaxError(function (e, xhr, settings) {
        "use strict";
        if (settings
                && settings.url
                && settings.url.indexOf("/api/client/log") === -1) {
            var error = "file: ";
            if (xhr) {
                if (xhr.status && xhr.responseText) {
                    error += "; status: " + xhr.status;
                    error += "; response: " + xhr.responseText;
                    window.logger.logError(error);
                }
            }
        }
    });
    /*jslint unparam: false*/
    $(window).error(function (e) {
        "use strict";
        var error = "";
        if (e.originalEvent) {
            if (e.originalEvent.filename
                    && e.originalEvent.lineno
                    && e.originalEvent.message) {
                error += "file: " + e.originalEvent.filename;
                error += "; line: " + e.originalEvent.lineno;
                error += "; message: " + e.originalEvent.message;
                window.logger.logError(error);
            }
        }
    });
}());