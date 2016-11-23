/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global viewModels,configuration*/

(function () {
    "use strict";
    window.businessViewModel = function (business) {

        business = business || {};

        var self = this, i,
            location = business.locations
            && business.locations.length > 0 ? business.locations[0] : {};

        self.businessBingId = location.business_id;

        self.cuisine = location.cuisine === undefined ? "" : location.cuisine.replace(new RegExp(',', 'g'), ', ');
        self.name = business.name || "";
        self.website = business.web_site || "";
        self.address = location.address1 || "";
        self.city = location.city || "";
        self.state = location.state || "";
        self.zip = location.zip || "";
        self.country = location.country_region || "";
        self.phoneNumber = location.phone_number || "";
        self.latitude = location.latitude || "";
        self.longitude = location.longitude || "";
        self.isOnlineDeal = self.address === '' ? true : false;
        self.getCompleteAddress = function () {
            var completeAddress = "";
            if (self.address !== "") {
                completeAddress = self.address + ", ";
            }

            if (self.city !== "") {
                completeAddress += self.city + ", ";
            }

            if (self.state !== "") {
                completeAddress += self.state + " ";
            }

            if (self.zip !== "") {
                completeAddress += self.zip;
            }

            return completeAddress;
        };
        self.bingMapsUrl = configuration.bingMaps.url +
            (self.businessBingId ? '&ss=ypid.' +
                self.businessBingId : '&lvl=15&cp=' +
                    self.latitude + '~' +
                    self.longitude + '&q=' +
                    self.name.replace('&', ''));

        self.localPlacesUrl = 'https://www.bing.com/local?lid=' + self.businessBingId;

        self.getMapUri = function () {
            return "http://www.bing.com/maps/Default.aspx?" +
                "where1=" + encodeURIComponent(self.getCompleteAddress()) +
                "&cp=" + self.latitude + '~' + self.longitude;
        };
    };
}());