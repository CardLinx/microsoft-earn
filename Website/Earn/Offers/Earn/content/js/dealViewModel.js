/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    ko,Math,
            viewModels,configuration*/


(function () {
    "use strict";
    window.dealViewModel = function (deal) {
        var self = this;
        self.isValid = deal && deal.id && (deal.business != null) ? true : false;
        self.imageUrl = deal.image_url;

        if (self.isValid) {
            self.business = new businessViewModel(deal.business);
            self.id = deal.id;
        } 
    };
}());