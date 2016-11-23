/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    "use strict";
    var client = "EARN_WEBSITE",
        commerceApiTimeout = 45000,
        commerceServerBaseUrl = "https://TODO_YOUR_URL_HERE/api/commerce/",
        secureCommerceServerBaseUrl = "https://TODO_YOUR_URL_HERE/",
        userServicesBaseUrl = "https://TODO_YOUR_URL_HERE/",
        apis = {
            commerceServer: {
                cards: commerceServerBaseUrl + "cards",
                cardsV2: commerceServerBaseUrl + "v2/cards",
                userTokenEndpointV2: commerceServerBaseUrl + "v2/securetokens/cards?operation=create&eventid=",
                redemptionEndpoint: commerceServerBaseUrl + "redemptionhistory",
                createReferralCodeEndpoint: commerceServerBaseUrl + "v2/referraltypes?rewardrecipient=",
                referralReportingEndpoint: commerceServerBaseUrl + "v2/referrals"
            },
            secureServer: {
                addCardViewEndpointV3: secureCommerceServerBaseUrl + "addcardv3/",
                addCardViewSettingsPageEndpointV2: secureCommerceServerBaseUrl + "AddCardV3/AddCardSettings"
            },
            userServices: {
                userInfo: userServicesBaseUrl + "api/userInfo",
                updateEmail: userServicesBaseUrl + "api/userInfo/updateEmail",
                updatePhoneNumber: userServicesBaseUrl + "api/userInfo/updatePhoneNumber",
                emailconfirmationstatus: userServicesBaseUrl + "api/userInfo/emailConfirmationStatus",
                updateNotificationPreferences: userServicesBaseUrl + "api/userInfo/updatenotificationpreference",
            },
            supportService: {
                support: '/api/support'
            },
            bouxAnalytics: "/api/analytics"
        },
    socialPlugin = {
        facebookShareUrlProd: "https://www.facebook.com/dialog/feed?app_id=993196157371603",
        twitterShareUrl: "https://twitter.com/share",
        pinterestShareUrl: "http://pinterest.com/pin/create/button/",
        emailShareUrl: "/offers/email/share"
    },
    messagingProtocolPrefix = "message:",
    analytics = {
        enableOmniture: window.location.hostname.indexOf("bing.com") > 0 && !(window.session && window.session.suppressAnalytics),
        enableBoAnalytics: false,
        logToConsole: window.location.hostname.indexOf("bing.com") < 0,
        enableEarnAnalytics: true,
        logSendingInterval: 2000
    },
     queryStrings = {
         manageSettings: "managesettings",
         pageAction: "ua",
         category: "category",
         sort: "sort",
         view: "view",
         location: "location",
         query: "query",
         analytics: {
             flight: "earn_fl",
             correlation: "boc",
             boABFlight: "boab",
             campaignName: "cmp_name",
             campaignSource: "cmp_source",
             campaignReferrer: "cmp_ref"
         },
         reward: {
             referralUrl: 'https://earn.microsoft.com',
             referredcode: "borrfc"
         }
     },
    cookies = {
        campaignReferrer: {
            //do not change the name
            name: queryStrings.analytics.campaignReferrer,
            expiration: 1 / 24
        },
        correlation: {
            //do not change the name
            name: queryStrings.analytics.correlation,
            expiration: 1 / 24
        },
        flight: {
            //do not change the name
            name: queryStrings.analytics.flight,
            expiration: 1 / 24
        },
        interstitial: {
            name: "bo_is",
            expiration: 365
        },
        location: {
            name: "bo_loc",
            expiration: 100
        },
        subscribed: {
            name: "bo_cta",
            expiration: 100
        },
        token: {
            name: "bo_token",
            expiration: 1 / 24
        },
        sid: {
            name: "bo_sid",
            expiration: 1 / 24
        },
        session: {
            //do not change the name
            name: "bos"
        },
        recentlyViewedHistory: {
            name: 'bo_rehist',
            expiration: 30
        },
        reward: {
            uuid: {
                name: "bouuid",
                expiration: 365
            },
            referredcode: {
                name: "borrfc"
            }
        },
        boABFlight: {
            name: queryStrings.analytics.boABFlight
        },
        bouxAnalytics: {
            browser_id: {
                name: 'earn_bid',
                expiration: 3650
            },
            session_id: {
                name: 'earn_sid',
                expiration: 0.007
            },
            newuser: {
                name: 'earn_nuser',
                expiration: 0.007
            }
        },
        remindLaterNoCardsNotification: {
            name: 'remind_later',
            expiration: 3
        },
        showUserLocation: {
            name: 'show_user_location',
            expiration: 0.007
        }
    },
    commands = {
        receiveCommands: {
            cancelButtonClicked: "cancel_button_click",
            cardSuccessfullyAdded: "card_added",
            userTokenExpired: "user_token_expired",
            goAhead: "goahead"
        },
        sendCommands: {
            userTokenUpdated: "user_token",
            parentUriUpdated: "parent_uri",
            parentSubmitClicked: "submit_clicked"
        }
    },
    bingMaps = {
        url: "http://www.bing.com/maps/Default.aspx?encType=1&v=2&style=r&mkt=en-us&FORM=LLDP",
        options: {
            credentials: "AgUyB7_Y6tTgEe1ywdRF24YxN16cVNUCXCzjdIRniISd5rtBJwkSvozrv9oMd-qi",
            showDashboard: true,
            showBreadcrumb: false,
            showMapTypeSelector: false,
            disableBirdseye: true,
            showScalebar: false,
            zoom: 15,
            enableSearchLogo: false
        }
    },
    popupNotifications = {
        showupDelay: 1000,
    },
    cardLinkedDmas = [{
        name: "seattle",
        boundingBox: {
            northLatitude: 48.152344,
            southLatitude: 46.905246,
            westLongitude: -123.239565,
            eastLongitude: -121.83722
        }
    }, {
        name: "boston",
        boundingBox: {
            northLatitude: 42.7006,
            southLatitude: 41.9075,
            westLongitude: -71.6236,
            eastLongitude: -70.4145
        }
    }, {
        name: "phoenix",
        boundingBox: {
            northLatitude: 33.8736,
            southLatitude: 32.9278,
            westLongitude: -112.6092,
            eastLongitude: -111.1120
        }
    }];

    window.configuration = {
        socialPlugin: socialPlugin,
        client: client,
        cookies: cookies,
        commerceApiTimeout: commerceApiTimeout,
        messagingProtocolPrefix: messagingProtocolPrefix,
        commands: commands,
        analytics: analytics,
        queryStrings: queryStrings,
        bingMaps: bingMaps,
        apis: apis,
        cardLinkedDmas: cardLinkedDmas,
        popupNotifications: popupNotifications
    };
}());