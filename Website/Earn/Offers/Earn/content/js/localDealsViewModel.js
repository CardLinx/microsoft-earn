/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {

    window.localDealsViewModel = function (parent, LocalDealsServerModel, TopDealsServerModel, CurrentState, SortOrder, SortBy) {
        var self = this;

        CurrentState = CurrentState || cookie.readCookie('earn_loc') || 'wa';
        self.deals = ko.observableArray([]);
        self.topDeals = ko.observableArray([]);
        self.selectedState = ko.observable(CurrentState);
        self.selectedSortBy = ko.observable(SortBy || 'City');
        self.selectedSortOrder = ko.observable(SortOrder || "asc");
        self.bingMapView = new window.bingMapViewModel(self, parent.page === 'restaurants', parent.page === 'places to earn');
        self.selectedSortBy.subscribe(function () {
            var sortBy = self.selectedSortBy();
            if (sortBy) {
                window.location.search = 'sortBy=' + sortBy + '&sortOrder=' + self.selectedSortOrder();
            }
        });

        self.selectedSortByClicked = function (sortByName) {
            if (self.selectedSortBy() === sortByName) {
                if (self.selectedSortOrder() === 'asc') {
                    self.selectedSortOrder('dsc');
                } else {
                    self.selectedSortOrder('asc');
                }

                window.location.search = 'sortBy=' + sortByName + '&sortOrder=' + self.selectedSortOrder();
            } else {
                self.selectedSortBy(sortByName);
            }
        };

        self.selectedState.subscribe(function () {
            var selectedCode = self.selectedState();
            if (selectedCode) {
                cookie.writeCookie('earn_loc', selectedCode, 365);
                window.location.reload();
            }
        });

        function initModel() {
            if (LocalDealsServerModel) {
                $.each(LocalDealsServerModel, function (index, item) {
                    self.deals.push(new dealViewModel(item));
                });
            }

            if (TopDealsServerModel) {
                $.each(TopDealsServerModel, function (index, item) {
                    self.topDeals.push(new dealViewModel(item));
                });
            }
        }



        initModel();
    };
}());