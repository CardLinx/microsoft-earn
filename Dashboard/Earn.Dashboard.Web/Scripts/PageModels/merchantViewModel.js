/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {

    window.offerViewModel = function (data) {
        var self = this;
        self.id = ko.observable(data.id);
        //self.providerId = ko.observable(data.provider_id);
        //self.providerName = ko.observable(data.provider_name);
        self.discount = ko.observable(data.discount);
        self.title = ko.observable(data.title);
        self.active = ko.observable(data.is_active);

        self.clone = function () {
            var result = new offerViewModel({
                id: self.id(),
                discount: self.discount(),
                title: self.title(),
                is_active: self.active()
            });

            return result;
        }
    };

    window.merchantViewModel = function (data) {
        var self = this;

        if (data.extended_attributes === undefined)
        {
            data.extended_attributes = {};
        }

        self.id = ko.observable(data.id);
        //self.providerId = ko.observable(data.provider_id);
        self.name = ko.observable(data.name);
        self.url = ko.observable(data.url);
        self.phone = ko.observable(data.phone_number);
        self.active = ko.observable(data.is_active);
        self.rank = ko.observable(data.rank);
        //self.location = ko.observable(data.location);

        self.payments = ko.observableArray([]);
        self.visa = ko.observableArray([]);
        self.mc = ko.observableArray([]);
        self.mcLocations = ko.observableArray([]);
        self.amex = ko.observableArray([]);

        self.validAmex = ko.observable(data.extended_attributes.ValidAmex === "true");
        self.validVisa = ko.observable(data.extended_attributes.ValidVisa === "true");
        self.validMasterCard = ko.observable(data.extended_attributes.ValidMasterCard === "true");
        self.validLocation = ko.observable(data.extended_attributes.ValidLocation === "true");

        self.visaMids = ko.observable("");
        self.mcMids = ko.observable("");
        self.mcLocationsStr = ko.observable("");
        self.amexMids = ko.observable("");

        // string to payment
        self.arrToVisaPayment = function (arr) {
            if (arr.length == 2) {
                return {
                    processor: 3,
                    paymentMids: { VisaMid: arr[0], VisaSid: arr[1] }
                };
            } else {
                return null;
            }
        }

        self.arrToMCPayment = function (arr) {
            if (arr.length == 2) {
                return {
                    processor: 4,
                    paymentMids: { AcquiringICA: arr[0], AcquiringMid: arr[1] }
                };
            } else {
                return null;
            }
        }

        self.arrToMCLocations = function (arr) {
            if (arr.length == 1) {
                return {
                    processor: 4,
                    paymentMids: { LocationID: arr[0] }
                };
            } else {
                return null;
            }
        }

        self.arrToAmexPayment = function (arr) {
            if (arr.length == 1) {
                return {
                    processor: 2,
                    paymentMids: { SENumber: arr[0] }
                };
            } else {
                return null;
            }
        }

        self.strToPayments = function (str, covertor) {
            var result = [];
            var pairs = str.split("|");
            for (var i = 0; i < pairs.length; i++) {
                if (pairs[i] == "") {
                    continue;
                }

                var values = pairs[i].split(";");
                var p = covertor(values);
                if (p != null) {
                    result.push(p);
                }
            }
            return result;
        }

        // payment to string
        self.visaToStr = function (payment) {
            return payment.paymentMids.VisaMid + ";" + payment.paymentMids.VisaSid;
        }

        self.mcToStr = function (payment) {
            if (payment.paymentMids.AcquiringICA !== undefined && payment.paymentMids.AcquiringMid !== undefined) {
                return payment.paymentMids.AcquiringICA + ";" + payment.paymentMids.AcquiringMid;
            } else {
                return null;
            }
        }

        self.mcLocationsToStr = function (payment) {
            if (payment.paymentMids.LocationID !== undefined) {
                return payment.paymentMids.LocationID;
            } else {
                return null;
            }
        }

        self.amexToStr = function (payment) {
            return payment.paymentMids.SENumber;
        }

        self.paymentToStr = function (payment, convertor) {
            var result = "";
            for (var i = 0; i < payment.length; i++) {
                var str = convertor(payment[i]);
                if (str === "") {
                    continue;
                }

                result += str;

                if (i != payment.length - 1) {
                    result += "|";
                }
            }

            return result
        }

        self.ProcessPayments = function () {
            self.payments([]);
            function processPayment(arr) {
                $.each(arr, function (i, payment) {
                    self.payments.push({
                        processor: payment.processor,
                        paymentMids: payment.paymentMids
                    });
                });
            };

            processPayment(self.visa());
            processPayment(self.mc());
            processPayment(self.mcLocations());
            processPayment(self.amex());
        }

        self.visaMids.subscribe(function (newVal) {
            self.visa(self.strToPayments(newVal, self.arrToVisaPayment));
            self.ProcessPayments();
        });

        self.mcMids.subscribe(function (newVal) {
            self.mc(self.strToPayments(newVal, self.arrToMCPayment));
            self.ProcessPayments();
        });

        self.mcLocationsStr.subscribe(function (newVal) {
            self.mcLocations(self.strToPayments(newVal, self.arrToMCLocations));
            self.ProcessPayments();
        });

        self.amexMids.subscribe(function (newVal) {
            self.amex(self.strToPayments(newVal, self.arrToAmexPayment));
            self.ProcessPayments();
        });

        $.each(data.payments, function (i, payment) {
            if (payment && payment.payment_mids) {

                var p = {
                    processor: payment.processor,
                    paymentMids: payment.payment_mids
                };

                switch (payment.processor) {
                    case 3:
                        self.visa.push(p);
                        break;
                    case 4:
                        if (p.paymentMids.LocationID !== undefined) {
                            self.mcLocations.push(p);
                        } else {
                            self.mc.push(p);
                        }
                        break;
                    case 2:
                        self.amex.push(p);
                        break;
                }
            }
        });

        self.visaMids(self.paymentToStr(self.visa(), self.visaToStr));
        self.mcMids(self.paymentToStr(self.mc(), self.mcToStr));
        self.mcLocationsStr(self.paymentToStr(self.mcLocations(), self.mcLocationsToStr))
        self.amexMids(self.paymentToStr(self.amex(), self.amexToStr));

        var location = data.location || {};
        self.address = ko.observable(location.address);
        self.city = ko.observable(location.city);
        self.state = ko.observable(location.state);
        self.zip = ko.observable(location.zip);
        self.latitude = ko.observable(location.latitude);
        self.longitude = ko.observable(location.longitude);
        self.locationText = ko.computed(function () {
            var locationText = "";
            if (self.address()) {
                locationText = self.address();
            }
            if (self.city()) {
                locationText += (locationText ? ', ' : '') + self.city();
            }
            if (self.state()) {
                locationText += (locationText ? ', ' : '') + self.state();
            }
            if (self.zip()) {
                locationText += (locationText ? ', ' : '') + self.zip();
            }
            return locationText;
        });

        self.clone = function () {
            var rawData = {
                id: self.id(),
                name: self.name(),
                url: self.url(),
                phone_number: self.phone(),
                is_active: self.active(),
                payments: [],
                rank: self.rank(),
                location: {
                    address: self.address(),
                    city: self.city(),
                    state: self.state(),
                    zip: self.zip(),
                    latitude: self.latitude(),
                    longitude: self.longitude()
                },
                extended_attributes: {
                    ValidAmex: self.validAmex().toString(),
                    ValidVisa: self.validVisa().toString(),
                    ValidMasterCard: self.validMasterCard().toString(),
                    ValidLocation: self.validLocation().toString()
                }
            };

            $.each(self.payments(), function (i, payment) {
                rawData.payments.push({
                    processor: payment.processor,
                    payment_mids: payment.paymentMids,
                });
            });

            var result = new merchantViewModel(rawData);
            return result;
        }
    };
}());