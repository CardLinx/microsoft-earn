/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    window.contactInfoViewModel = function (parent, UserInfoServerModel, isEnroll) {

        var self = this;
        UserInfoServerModel = UserInfoServerModel || {};

        self.email = ko.observable(UserInfoServerModel.email);
        self.uneditableEmail = ko.observable(UserInfoServerModel.email);

        self.phone = ko.observable(UserInfoServerModel.phone_number);
        self.uneditablePhone = ko.observable(UserInfoServerModel.phone_number);

        self.emailConfirmationPending = ko.observable(false);
        self.editMode = ko.observable(false);

        self.showSpinner = ko.observable(false);
        self.error = ko.observable();

        var hasTransactionnNotificationMediumSetting = UserInfoServerModel.hasOwnProperty('information') && UserInfoServerModel.information.hasOwnProperty('preferences') && UserInfoServerModel.information.preferences.hasOwnProperty('transaction_notification_medium');
        var sendToEmail = hasTransactionnNotificationMediumSetting && (UserInfoServerModel.information.preferences.transaction_notification_medium == 1 || UserInfoServerModel.information.preferences.transaction_notification_medium == 3);
        var sendToPhone = hasTransactionnNotificationMediumSetting && (UserInfoServerModel.information.preferences.transaction_notification_medium == 2 || UserInfoServerModel.information.preferences.transaction_notification_medium == 3);

        self.sendEarnConfirmationsToEmail = ko.observable(sendToEmail || isEnroll);
        self.uneditableSendEarnConfirmationsToEmail = ko.observable(sendToEmail);

        self.sendEarnConfirmationsToPhone = ko.observable(sendToPhone);
        self.uneditableSendEarnConfirmationsToPhone = ko.observable(sendToPhone);

        self.selectedCheckBox = ko.observable(self.sendEarnConfirmationsToPhone());

        self.sendEarnConfirmationsToPhone.subscribe(
            function (newValue) {
                if (!newValue) {
                    self.selectedCheckBox(false);
                }
            });

        self.phone.subscribe(
            function (newValue) {
                if (!self.uneditablePhone() || self.uneditablePhone().trim().length == 0)
                {
                    var isValid = (window.userServices.validatePhoneNumber(self.phone()));
                    self.sendEarnConfirmationsToPhone(isValid);
                    self.selectedCheckBox(isValid);
                }
            });

        self.validateFields = function () {
            self.error('');
            if (!window.userServices.validateEmail(self.email())) {
                self.error('Enter a valid email address.');
                return false;
            }

            var userHasValidPhone = false;
            if (self.phone() && self.phone().trim().length > 0) {
                if (!window.userServices.validatePhoneNumber(self.phone())) {
                    self.error('Enter a valid phone number.');
                    return false;
                }

                userHasValidPhone = true;

            }

            if (isEnroll) {
                self.sendEarnConfirmationsToPhone(userHasValidPhone);
                self.uneditableSendEarnConfirmationsToPhone(userHasValidPhone);
                self.selectedCheckBox(userHasValidPhone);
            }

            if (self.sendEarnConfirmationsToPhone() && !self.selectedCheckBox()) {
                self.error('You need to agree to the authorization.');
                return false;
            } else if (self.sendEarnConfirmationsToPhone() && self.selectedCheckBox() && !userHasValidPhone) {
                self.error('Enter a valid phone number.');
                return false;
            }

            return true;
        };

        var undoChanges = function (editmode) {
            if (!editmode) {
                self.phone(self.uneditablePhone());
                self.email(self.uneditableEmail());
                self.sendEarnConfirmationsToEmail(self.uneditableSendEarnConfirmationsToEmail());
                self.sendEarnConfirmationsToPhone(self.uneditableSendEarnConfirmationsToPhone());
            }
        }

        self.editMode.subscribe(undoChanges);

        var loadEmailStatus = function () {
            $.when(userServices.getEmailStatus())
                .done(function (status) {
                    if (status && status.email_to_confirm) {
                        if (status.waiting_for_confirmation) {
                            self.emailConfirmationPending(true);
                            self.email(status.email_to_confirm);
                            self.uneditableEmail(status.email_to_confirm);
                        }
                    }
                }).fail(function () {
                });
        };

        self.editClicked = function () {
            self.editMode(true);
        };

        self.cancelClicked = function () {
            self.editMode(false);
        };

        self.saveClicked = function (isEnroll, callback) {
            self.error('');

            if (!window.userServices.validateEmail(self.email())) {
                self.error('Enter a valid email address.');
                return;
            }

            var updatePhoneRequired = false;

            var userHasValidPhone = false;
            if (self.phone() && self.phone().trim().length > 0) {
                if (!window.userServices.validatePhoneNumber(self.phone())) {
                    self.error('Enter a valid phone number.');
                    return;
                }

                userHasValidPhone = true;
            }

            if (self.sendEarnConfirmationsToPhone() && !self.selectedCheckBox()) {
                self.error('You need to agree to the authorization.');
                return;
            } else if (self.sendEarnConfirmationsToPhone() && self.selectedCheckBox() && !userHasValidPhone) {
                self.error('Enter a valid phone number.');
                return;
            }

            self.showSpinner(true);

            var emailDone = false;
            var phoneDone = false;
            var notificationsDone = false;

            $.when(userServices.updateEmail(self.email())).done(function (msg) {
                emailDone = true;
                self.uneditableEmail(self.email());
                if (msg === 'confirm') {
                    self.emailConfirmationPending(true);
                } else {
                    self.emailConfirmationPending(false);
                }
                if (phoneDone && notificationsDone) {
                    self.showSpinner(false);
                    self.editMode(false);
                    if (isEnroll && callback) {
                        callback();
                    }
                }
            }).fail(function (error) {
                self.error(error);
                emailDone = true;
                if (phoneDone && notificationsDone) {
                    self.showSpinner(false);
                }
            });

            var deletePhone = !self.phone() || self.phone().trim().length == 0;

            $.when(userServices.updatePhoneNumber(self.phone(), deletePhone)).done(function (msg) {
                phoneDone = true;
                self.uneditablePhone(self.phone());
                if (emailDone && notificationsDone) {
                    self.showSpinner(false);
                    self.editMode(false);

                    if (isEnroll && callback) {
                        callback();
                    }
                }
            }).fail(function (error) {
                self.error("An error occured saving the phone number.");
                phoneDone = true;
                self.uneditablePhone(self.phone());
                if (emailDone && notificationsDone) {
                    self.showSpinner(false);
                }
            });


            $.when(userServices.updateNotificationPreference(self.sendEarnConfirmationsToEmail(), self.sendEarnConfirmationsToPhone())).done(function (msg) {
                notificationsDone = true;

                self.uneditableSendEarnConfirmationsToPhone(self.sendEarnConfirmationsToPhone());
                self.uneditableSendEarnConfirmationsToEmail(self.sendEarnConfirmationsToEmail());

                if (emailDone && phoneDone) {
                    self.showSpinner(false);
                    self.editMode(false);

                    if (isEnroll && callback) {
                        callback();
                    }
                }

            }).fail(function (error) {
                self.error("An error occured saving the notification preferences.");
                notificationsDone = true;
                self.uneditablePhone(self.phone());
                if (emailDone && phoneDone) {
                    self.showSpinner(false);
                }
            });
        };

        if (!isEnroll) {
            loadEmailStatus();
        }
    };
}());

