/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
window.notification = function (subject, message, remindInterval) {

    var self = this;
    notification = notification || {};

    self.subject = subject;
    self.notificationId = self.subject.replace(/\s+/g, '').toLowerCase();
    self.cookieName = "remindlater_" + self.notificationId;
    self.message = message;
    self.remindInterval = remindInterval

    self.show = function () {
        var isRemindLaterCookieExists = window.cookie.readCookie(self.cookieName);
        if (!isRemindLaterCookieExists) {
            $('#' + self.notificationId).fadeIn("slow");
        }
    };

    self.hide = function () {
        $('#' + self.notificationId).fadeOut("slow");
    };

    self.remindLater = function () {
        self.hide();
        window.cookie.writeCookie(self.cookieName, true, self.remindInterval);
    };
};