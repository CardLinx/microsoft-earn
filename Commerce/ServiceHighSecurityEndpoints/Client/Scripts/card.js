/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    $, window,
            configuration*/

(function () {
    "use strict";
    var cardInfo = function (
        nameOnCard,
        cardNumber,
        cardExpirationYear,
        cardExpirationMonth,
        cardBrand
    ) {
        var self = this;

        self.nameOnCard = nameOnCard;
        self.cardNumber = cardNumber;
        self.cardExpirationMonth = cardExpirationMonth;
        self.cardExpirationYear = cardExpirationYear;
        self.cardBrand = cardBrand;

        this.getSerializedCardInfo = function () {
            return JSON.stringify({
                "name_on_card": self.nameOnCard,
                "number": self.cardNumber,
                "expiration": self.cardExpirationYear + "-" + self.cardExpirationMonth,
                "card_brand": self.cardBrand
            });
        };

        this.validateCard = function () {
            var
                checkCardExpiry = function () {
                    var
                        month = self.cardExpirationMonth,
                        year = self.cardExpirationYear,
                        now = new Date();

                    if (!month || !year) {
                        return false;
                    }

                    return (parseInt(month, 10) >= (now.getMonth() + 1) ||
                        parseInt(year, 10) >= now.getFullYear());
                },

                checkCardType = function () {
                    var
                        digit,
                        number = self.cardNumber,
                        cardType = self.cardBrand;

                    // minimum of 13 digits for amex card
                    if (!number || number.length < 13) {
                        return false;
                    }
                    digit = number.charAt(0);
                    /*ignore jslint start*/
                    switch (cardType) {
                        /*  American express cards disabled temporarily as they don't work.
                        case "AmericanExpress":
                            return (digit === "3");*/
                        case "Visa":
                            return (digit === "4");
                        case "MasterCard":
                            return (digit === "5");
                        default:
                            return false;
                    }
                    /*ignore jslint end*/
                },

                runMod10 = function () {
                    var
                        digit,
                        number = self.cardNumber,
                        luhnModifiedValues = [0, 2, 4, 6, 8, 1, 3, 5, 7, 9],
                        checksum = 0,
                        modifyDigits = false, count;

                    if (!number) {
                        return false;
                    }

                    for (count = number.length - 1; count >= 0; count = count - 1) {
                        digit = number.charAt(count);

                        if (modifyDigits === false) {
                            checksum += parseInt(digit, 10);
                        } else {
                            checksum += luhnModifiedValues[digit];
                        }

                        modifyDigits = !modifyDigits;
                    }

                    return (checksum % 10 === 0);
                },

                checkNameOnCard = function () {
                    var name = $.trim(self.nameOnCard);
                    return (name && name.length > 0);
                };

            return (checkCardType() &&
                runMod10() &&
                checkNameOnCard() &&
                checkCardExpiry());
        };

        return this;
    };

    window.card = {
        cardInfo: cardInfo
    };
}());