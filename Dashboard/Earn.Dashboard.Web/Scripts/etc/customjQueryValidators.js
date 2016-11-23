/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
function validateMidsSequence(value, midsInPair) {
    var isValid = true;

    if (value.length > 0) {
        var pairs = value.split("|");
        for (var i = 0; i < pairs.length; i++) {
            if (pairs[i] == "") {
                continue;
            }

            var mids = pairs[i].split(";");
            if (mids.length == midsInPair) {
                isValid &= true;

                for (var j = 0; j < mids.length; j++) {
                    isValid &= $.isNumeric(mids[j]);
                }

            } else {
                isValid &= false;
            }
        }
    }

    return isValid;
}

jQuery.validator.addMethod("visaMids", function (value, element) {
    return validateMidsSequence(value, 2);
}, "Please enter the Visa MIDS in following format: VisaMid;VisaSid|VisaMid;VisaSid... ");

jQuery.validator.addMethod("mcMids", function (value, element) {
    return validateMidsSequence(value, 2);
}, "Please enter the Master Card MIDS in following format: AcquiringICA;AcquiringMid... ");

jQuery.validator.addMethod("mcLocations", function (value, element) {
    return validateMidsSequence(value, 1);
}, "Please enter the Master Card Locations in following format: LocationId|LocationId... ");

jQuery.validator.addMethod("amexMids", function (value, element) {
    return validateMidsSequence(value, 1);
}, "Please enter the AMEX MIDS in following format: SENumber|SENumber... ");

jQuery.validator.addMethod("noSpace", function (value, element) {
    return $.trim(value) ? true : false;
}, "No space please and don't leave it empty");