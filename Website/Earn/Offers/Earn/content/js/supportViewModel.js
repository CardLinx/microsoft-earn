/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    window.supportViewModel = function (parent, UserInfoServerModel) {
        var self = this;

        UserInfoServerModel = UserInfoServerModel || {};

        self.assistWithOptions = ko.observableArray([
            "I’m missing Earn Credits",
            "I’m missing my statement credit from redeeming my Earn Credits",
            "I can’t add credit/debit card",
            "I can’t sign in",
            "I had a problem with a recent Earn promotion",
            "I did not receive my $5 bonus Earn Credits for signing up for the program",
            "I did not receive my $5 bonus Earn Credits for referring a friend",
            "My question is not listed here"
        ]);

        self.cardTypeOptions = ko.observableArray([
            "Credit Card",
            "Debit Card"
        ]);

        self.cardBrandOptions = ko.observableArray([
            "Visa",
            "MasterCard"
        ]);

        self.didYourEnterPINOptions = ko.observableArray([
            "Yes",
            "No"
        ]);

        self.fullName = ko.observable();
        self.email = ko.observable(UserInfoServerModel.email);
        self.assistWith = ko.observable();
        self.cardType = ko.observable();
        self.cardBrand = ko.observable();
        self.didYourEnterPIN = ko.observable();
        self.last4digits = ko.observable();
        self.merchantName = ko.observable();
        self.merchantAddress = ko.observable();
        self.purchaseDate = ko.observable();
        self.amount = ko.observable();
        self.promotion = ko.observable();
        self.details = ko.observable();
        self.error = ko.observable();
        self.showSpinner = ko.observable(false);
        self.requestSubmitted = ko.observable(false);

        self.showDidYouEnterPIN = ko.observable(false);
        self.cardType.subscribe(function (newValue) {
            self.showDidYouEnterPIN(newValue === 'Debit Card');
        })

        self.submitClicked = function () {
            self.error('');

            if (!window.userServices.validateEmail(self.email())) {
                self.error('Enter a valid email address.');
                return;
            }

            if (!self.fullName() || self.fullName().length < 1) {
                self.error('Please include your full name');
                return;
            }

            if (!self.assistWith() || self.assistWith().length < 1) {
                self.error('What would you like assistance with?');
                return;
            }

            if (self.showDidYouEnterPIN() && (!self.didYourEnterPIN() || self.didYourEnterPIN().length < 1)) {
                self.error('Did you enter a PIN when you made the purchase?');
                return;
            }

            var obj = {
                full_name: self.fullName(),
                email: self.email(),
                assistance_with: self.assistWith(),
                last_4_digits: self.last4digits(),
                card_type: self.cardType(),
                card_brand: self.cardBrand(),
                pin_entered: self.didYourEnterPIN(),
                merchant_name: self.merchantName(),
                merchant_address: self.merchantAddress(),
                purchase_date: self.purchaseDate(),
                purchase_amount: self.amount(),
                promotion: self.promotion(),
                details: self.details()

            };

            self.showSpinner(true);

            $.ajax({
                url: configuration.apis.supportService.support,
                method: 'POST',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(obj)
            }).done(function () {
                self.showSpinner(false);
                self.requestSubmitted(true);
            }).error(function (errorMsg) {
                self.error(errorMsg);
                self.showSpinner(false);
            });
        };

        $('.last4digits').keypress(window.helper.allowNumeric);
    };
}());