/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
window.socialPluginViewModel = function (analyticsTracker, config) {
    var self = this;

    self.facebookPageShare = function () {
        if (analyticsTracker) {
            analyticsTracker.trackEvent('share.fb', 'social')
        }

        var url = configuration.socialPlugin.facebookShareUrlProd
            + "&picture=" + encodeURIComponent(config.image_url)
            + "&caption=" + encodeURIComponent(config.name)
            + "&description=" + encodeURIComponent(config.description)
            + "&redirect_uri=" + encodeURIComponent(config.redirect_uri)
            + "&display=popup&show_error=false&link=" + encodeURIComponent(config.share_url)
            + "&name=" + encodeURIComponent(config.title);

        openWindow(url, "Facebook");
    };

    var openWindow = function (url, name, opts) {
        if (url) {
            var openedWindow,
                dimension = "height=580,width=640";

            if (helper.isMobileView()) {
                dimension = "height=400,width=300";
            }

            if (opts) {
                dimension += opts;
            }
            openedWindow = window.open(url, name, dimension);
            if (openedWindow) {
                openedWindow.focus();
            }
        }
    };

    self.pinterestShare = function () {
        if (analyticsTracker) {
            analyticsTracker.trackEvent('share.pinterest', 'social')
        }

        var url = configuration.socialPlugin.pinterestShareUrl +
                "?url=" + encodeURIComponent(config.share_url) +
                "&media=" + encodeURIComponent(config.image_url) +
                "&description=" + encodeURIComponent(config.title + ' - ' + config.description);

        openWindow(url, "Pinterest");
    };

    self.twitterPageShare = function () {
        if (analyticsTracker) {
            analyticsTracker.trackEvent('share.twitter', 'social')
        }

        var shareUrl = config.share_url;
        var url = configuration.socialPlugin.twitterShareUrl +
            "?text=" + encodeURIComponent(config.twitterTitle) +
            "&via=EarnByMicrosoft&data-related=EarnByMicrosoft&url=" + encodeURIComponent(shareUrl) +
            "&hashtags=Microsoft";

        openWindow(url, "Twitter");
    };

    self.emailShare = function () {
        if (analyticsTracker) {
            analyticsTracker.trackEvent('share.email', 'social')
        }

        window.location = "mailto:?subject=" + config.title + "&body=" + config.emailBody + encodeURIComponent("\r\n") + encodeURIComponent(config.share_url);
    }
};
