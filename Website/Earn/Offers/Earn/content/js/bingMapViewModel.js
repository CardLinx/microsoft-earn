/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    window.bingMapViewModel = function (parent, isRestaurantsPage, isPlacesToEarnPage) {
        var self = this;

        self.isMobileView = function () {
            return $(document).width() < 1449;
        };

        self.pinInfobox = null;
        self.bingMap = null;
        self.userLocationPin = null;

        if (isRestaurantsPage) {
            $(window).resize(function () {
                if (self.pinInfobox && self.pinInfobox.getOptions().visible) {
                    if (self.isMobileView()) {
                        self.bingMap.setView({ center: new Microsoft.Maps.Location(self.pinInfobox._location.latitude, self.pinInfobox._location.longitude) });
                    }

                    self.pinInfobox.setOptions(
                    {
                        showPointer: !self.isMobileView(),
                        offset: self.getPushpinOffset(),
                        visible: true,
                    });
                }
            });
        }

        self.init = function () {
            Microsoft.Maps.loadModule('Microsoft.Maps.Themes.BingTheme', { callback: bingModulesLoaded });

            function bingModulesLoaded() {
                var $elem = $(".bingmap-container");
                if ($elem[0]) {

                    self.bingMap = new Microsoft.Maps.Map($(".bingmap-container")[0],
                    {
                        credentials: configuration.bingMaps.options.credentials,
                        mapTypeId: Microsoft.Maps.MapTypeId.road,
                        zoom: 7,
                        showMapTypeSelector: false,
                        showBreadcrumb: false,
                        enableSearchLogo: false,
                        showDashboard: isRestaurantsPage,
                        showScalebar: isRestaurantsPage,
                        theme: new Microsoft.Maps.Themes.BingTheme(),
                    });

                    self.locateState(parent.selectedState());
                    self.addPushpins(parent.deals());

                    // disable scrolling map in mobile view
                    if (self.isMobileView()) {
                        Microsoft.Maps.Events.addHandler(self.bingMap, 'mousewheel', function (e) {
                            e.handled = true;
                            return true;
                        });
                    }

                    // show user location
                    if (cookie.readCookie(configuration.cookies.showUserLocation.name)) {
                        cookie.eraseCookie(configuration.cookies.showUserLocation.name);
                        self.locateUser();
                    }
                }
            }
        };

        self.locateState = function (state) {
            if (state == "wa") {
                self.bingMap.setView({ zoom: 9, center: new Microsoft.Maps.Location(47.61, -122.338) });
            } else if (state == "az") {
                self.bingMap.setView({ zoom: 10, center: new Microsoft.Maps.Location(33.461929, -112.093819) });
            } else if (state == "ma") {
                self.bingMap.setView({ zoom: 9, center: new Microsoft.Maps.Location(42.363949, -71.064262) });
            }
        };

        self.detectStateByCoordinates = function (lat, long) {
            var userState = null;
            if (long > -125 && long < -116.0 && lat < 49.5 && lat > 45) {
                userState = "wa";
            }
            else if (long > -115 && long < -107 && lat < 38 && lat > 30) {
                userState = "az";
            }
            else if (long > -73.7 && long < -69.7 && lat < 43 && lat > 41.1) {
                userState = "ma";
            }

            return userState;
        }

        self.getPushpinOffset = function () {
            var newLeft = (-1) * self.pinInfobox.getWidth() / 2;
            var offset = self.isMobileView() ? new Microsoft.Maps.Point(newLeft, 20) : new Microsoft.Maps.Point(0, 0);
            return offset;
        }

        self.addPushpins = function (deals) {

            function displayInfobox(e) {

                if (self.isMobileView()) {
                    self.bingMap.setView({ center: new Microsoft.Maps.Location(e.target._location.latitude, e.target._location.longitude) });
                }

                if (e.targetType == 'pushpin') {
                    self.pinInfobox.setLocation(e.target.getLocation());
                    self.pinInfobox.setOptions(
                        {
                            showPointer: !self.isMobileView(),
                            visible: true,
                            offset: self.getPushpinOffset(),
                            title: e.target.Title,
                            description: e.target.Address,
                            actions: [
                                {
                                    label: 'Details',
                                    eventHandler: function () { window.open(e.target.Url); }
                                }]
                        });
                }
            }

            self.bingMap.entities.clear();
            $.each(deals, function (index, item) {
                var pushpin = new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(item.business.latitude, item.business.longitude), null);
                pushpin.Title = item.business.name;
                pushpin.Address = item.business.address;
                pushpin.Url = item.business.localPlacesUrl;

                self.bingMap.entities.push(pushpin);

                // Add handler for the pushpin click event.
                Microsoft.Maps.Events.addHandler(pushpin, 'click', displayInfobox);
            });

            self.pinInfobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0),
            {
                visible: false,
                offset: new Microsoft.Maps.Point(0, 0),
                width: 300,
            });
            self.bingMap.entities.push(self.pinInfobox)
        };

        self.locateUser = function () {

            // user location found callback
            function userLocationFound(userLocation) {
                var coords = userLocation.position.coords;

                // determine user state
                var userState = self.detectStateByCoordinates(coords.latitude, coords.longitude);
                if (userState != parent.selectedState()) {
                    cookie.writeCookie(configuration.cookies.showUserLocation.name, userState, configuration.cookies.showUserLocation.expiration);
                    parent.selectedState(userState);
                    return;
                }

                self.bingMap.setView({
                    zoom: 13,
                    center: userLocation.center
                });

                // remove previous user location pin and area entities if they are exists
                var location = new Microsoft.Maps.Location(coords.latitude, coords.longitude);
                var uselLocationPinIndex = self.bingMap.entities.indexOf(self.userLocationPin);
                if (uselLocationPinIndex != -1) {
                    self.bingMap.entities.removeAt(uselLocationPinIndex);
                    self.bingMap.entities.removeAt(uselLocationPinIndex - 1);
                }

                self.userLocationPin = new Microsoft.Maps.Pushpin(location, { icon: "Offers\\Earn\\content\\images\\userlocation.png", anchor: new Microsoft.Maps.Point(14, 14) });
                self.bingMap.entities.push(self.userLocationPin);
            };

            // determine user location
            var geoLocationProvider = new Microsoft.Maps.GeoLocationProvider(self.bingMap);
            geoLocationProvider.getCurrentPosition({ successCallback: userLocationFound });
        }

        if (isRestaurantsPage || isPlacesToEarnPage) {
            self.init();
        }
    };
}());