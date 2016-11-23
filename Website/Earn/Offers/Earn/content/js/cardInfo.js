/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
window.cardInfo = function (card) {

    var self = this;
    card = card || {};

    self.id = card.id;
    self.last_four_digits = card.last_four_digits;
    self.card_brand = card.card_brand;
};

window.cardInfo.prototype.formattedCardNumber = function () {
    return "XXXXXXXXXXXX" + this.last_four_digits;
};