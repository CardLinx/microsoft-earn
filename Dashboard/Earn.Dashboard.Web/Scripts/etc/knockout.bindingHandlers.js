/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    ko.bindingHandlers.select2 = {
        init: function (element, valueAccessor, allBindings, data, context) {
            var options = ko.toJS(valueAccessor()) || {};

            //wait for foreach to render option elements
            setTimeout(function () {
                $(element).select2(options);
            }, 0);
        }
    };

    ko.bindingHandlers.date = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            var value = valueAccessor(), allBindings = allBindingsAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);
            var d = new Date(value);

            $(element).text(d.fullDateToString());
        }
    };

}());