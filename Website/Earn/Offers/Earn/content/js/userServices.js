/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    $,window,
            configuration,analytics*/
/*
    user services functionalities.
*/

(function () {
    "use strict";
    var request = {
        contentType: "application/json",
        get: "GET",
        post: "POST"
    },
        validateZipCode = function (zipCode) {
            return (zipCode && zipCode.length === 5 && /^[0-9]+$/.test(zipCode));
        },
        validateEmail = function (email) {
            return (email && /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email)); //ignore jslint
        },
        validatePhoneNumber = function (phoneNumber) {
            return (phoneNumber && /^\+?1?\s?[-./\s\\]?\s?\(?\d{3}\)?\s?[-./\s\\]?\s?\d{3}\s?[-./\s\\]?\s?\d{4}$/.test(phoneNumber)); //ignore jslint
        },

        //user services
        getUserInfo = function () {
            return $.Deferred(function (def) {
                $.ajax({
                    url: configuration.apis.userServices.userInfo,
                    type: request.get,
                    contentType: request.contentType
                }).done(function (userInfo) {
                    def.resolve(userInfo);
                }).fail(function () {
                    def.reject();
                });
            }).promise();
        },

        updateEmail = function (email) {
            return $.Deferred(function (def) {
                if (!validateEmail(email)) {
                    def.reject("email");
                } else {
                    $.ajax({
                        url: configuration.apis.userServices.updateEmail
                            + "?emailAddress=" + email,
                        type: request.post,
                        contentType: request.contentType
                    }).done(function (msg) {
                        if (msg && msg.toLowerCase() === "emailconfirmationrequired") {
                            def.resolve("confirm");
                        } else {
                            def.resolve();
                        }
                    }).fail(function (xhr) {
                        if (xhr && xhr.status) {
                            if (xhr.status === 409) {
                                //conflict email registered by other
                                def.reject("This email has already been registered with another account.");
                            } else {
                                def.reject('An error occured updating the email.');
                            }
                        } else {
                            def.reject('An error occured updating the email.');
                        }
                    });
                }
            }).promise();
        },
        updatePhoneNumber = function (phoneNumber, deletePhoneNumber) {
            return $.Deferred(function (def) {
                var deletePhone = (deletePhoneNumber) ? "&deletephone=true" : "";
                if (!validatePhoneNumber(phoneNumber) && !deletePhone) {
                    def.reject("phone");
                } else {
                    $.ajax({
                        url: configuration.apis.userServices.updatePhoneNumber
                            + "?phoneNumber=" + phoneNumber + deletePhone,
                        type: request.post,
                        contentType: request.contentType
                    }).done(function () {
                        def.resolve();
                    }).fail(function () {
                        def.reject();
                    });
                }
            }).promise();
        },
        getUserToken = function (referredCode, referrer) {
             var parentId = window.parentId,
                 referralQueryParam = referredCode ? "&referralcode=" + referredCode : "",
                 referrerQueryParam = referrer ? "&referrer=" + referrer : "";

             if (!parentId) {
                 parentId = helper.generateGuid();
             }

             return $.ajax({
                 type: "GET",
                 dataType: "json",
                 timeout: configuration.commerceApiTimeout,
                 url: configuration.apis.commerceServer.userTokenEndpointV2 + parentId + referralQueryParam + referrerQueryParam
             });
         },
        getEmailStatus = function () {
              return $.Deferred(function (def) {
                  $.ajax({
                      url: configuration.apis.userServices.emailconfirmationstatus,
                      type: request.get,
                      contentType: request.contentType
                  }).done(function (emailStatus) {
                      def.resolve(emailStatus);
                  }).fail(function () {
                      def.reject();
                  });
              }).promise();
          },
        updateNotificationPreference = function (sendToEmail, sendToPhone) {
            return $.Deferred(function (def) {

                var requestArgs = "notificationPreferences=none"
                if (sendToEmail && sendToPhone) {
                    requestArgs = "notificationPreferences=email&notificationPreferences=phone";
                } else if (sendToEmail && !sendToPhone) {
                    requestArgs = "notificationPreferences=email";
                } else if (!sendToEmail && sendToPhone) {
                    requestArgs = "notificationPreferences=phone";
                }

                $.ajax({
                    url: configuration.apis.userServices.updateNotificationPreferences + "?" + requestArgs,
                    type: request.post,
                    contentType: request.contentType
                }).done(function () {
                    def.resolve();
                }).fail(function () {
                    def.reject();
                });
            }).promise();
        };


    window.userServices = {
        validateZipCode: validateZipCode,
        validateEmail: validateEmail,
        validatePhoneNumber: validatePhoneNumber,
        getUserInfo: getUserInfo,
        updateEmail: updateEmail,
        updatePhoneNumber: updatePhoneNumber,
        getUserToken: getUserToken,
        getEmailStatus: getEmailStatus,
        updateNotificationPreference: updateNotificationPreference,
    };
}());