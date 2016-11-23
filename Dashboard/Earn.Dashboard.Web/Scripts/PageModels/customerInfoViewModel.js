/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {

    window.customerInfoViewModel = function (userIdentityName)
    {
        var self = this;

        self.user = userIdentityName;

        self.issueCreditsAmount = ko.observable();
        self.issueCreditsExplanation = ko.observable();

        self.enableComponents = ko.observable(true);

        self.customerFilter = ko.observable();

        self.foundCustomersTable;
        self.transactionHistoryTable;
        self.referralsTable;
        self.earnBurnLineItemsTable;
        self.earnBurnHistoryTable;

        self.notesTimeline = ko.observable(new notesTimelineViewModel());

        self.foundCustomers = [];
        self.customer = ko.observable({});
        self.customerCredits = ko.observable("$0.00");

        self.showCustomerAndCardInfo = ko.observable(false);
        self.showTabsWithData = ko.observable(false);
        self.showSpinnerInTransactionsTab = ko.observable(false);
        self.showSpinnerInEarnBurnLineItemsTab = ko.observable(false);
        self.showSpinnerInReferralsTab = ko.observable(false);
        self.showSpinnerInNotesTab = ko.observable(false);

        self.customer.subscribe(function (val) {
            self.showCustomerAndCardInfo(val != undefined);
            self.showTabsWithData(val != undefined);

            if (val != undefined)
            {
                self.showTabsWithData(true);
                self.getTransactionHistory();
                self.getEarnBurnLineItems();
                self.getReferrals();
                self.getNotes();
                $('.nav-tabs a[href="#tabs-transactionHistory"]').tab('show')

                self.updateUrlInBrowser();
            }
        })

        self.initUI = function () {
            $("#issue_credit_value").inputmask('currency',
                {
                    'allowMinus': true,
                    rightAlign: false,
                });

            self.resetIssueCreditValues();

            self.foundCustomersTable = new window.tableWidget(
                '#foundCustomers',
                window.datatableDefinitions.foundCustomersColumns,
                window.datatableDefinitions.foundCustomersRowCreateCallback);

            self.transactionHistoryTable = new window.extendedTableWidget(
                '#transactionHistoryDataTable',
                window.datatableDefinitions.earnBurnHistoryColumns,
                1, 1, 'desc', 50, true,
                undefined, undefined, undefined, window.datatableDefinitions.earnBurnHistoryRowCreateCallback);

            self.referralsTable = new window.extendedTableWidget(
                '#referralsDataTable',
                window.datatableDefinitions.referralsColumns,
                1, 4, 'desc', 50, true,
                undefined, undefined, undefined, window.datatableDefinitions.referralsRowCreateCallback, window.datatableDefinitions.referralsRowCreatedCallback);

            self.earnBurnLineItemsTable = new window.extendedTableWidget(
                '#earnBurnLineItemsDataTable',
                window.datatableDefinitions.earnBurnLineItemsColumns,
                1, 1, 'desc', 50, true,
                undefined, undefined, undefined, window.datatableDefinitions.earnBurnLineItemsRowCreateCallback, undefined);

            $('#issueCreditsForm').validate({
                debug: false,
                errorClass: 'text-danger',
                errorElement: 'span',
                rules: {
                    issueCreditValue: {
                        noSpace: true,
                        required: true
                    },
                    issueCreditExplanation: {
                        noSpace: true,
                        required: true,
                    },
                },
                messages: {
                    issueCreditValue: 'Credit value is required',
                    issueCreditExplanation: 'Explanation is required'
                }
            });

            $('#customerFilter').focus();
        };

        self.customerSelected = function ()
        {
            for (var i = 0; i < self.foundCustomers.length; i++) {
                if (self.foundCustomers[i].global_id == self.foundCustomersTable.selectedData[1]) {
                    self.customer(self.foundCustomers[i]);
                    break;
                }
            }
        }

        self.onEnterFindCustomer = function (d, e) {
            if (e.keyCode === 13)
            {
                var target = e.target;
                target.blur();
                self.findCustomer();
            }

            return true;
        };

        self.showAlertMessage = function (error) {
            if (error.status == 403) {
                $('.alert-warning').show();
            } else {
                var msg = error.responseText.replace(/\\r\\n/g, "<br />")

                $('.box-danger')
                    .find('p')
                    .html(msg);

                $('.box-danger').show();
            }
        };

        self.hideAlertMessages = function () {
            $('.alert').hide();
            $('.box-danger').hide();
        }

        // begin issue credits

        self.showIssueCreditsModal = function () {
            $('#issueCreditsModal').modal('show');
        }

        self.resetIssueCreditValues = function () {
            self.issueCreditsAmount('$00.00');
            self.issueCreditsExplanation('');
        }

        self.issueCredits = function () {
            var validator = $('#issueCreditsForm').validate();
            if (!validator.valid()) {
                return;
            }

            var requestParameters = {
                UserId: self.customer().global_id,
                Explanation: self.issueCreditsExplanation(),
                Amount: self.issueCreditsAmount().replace('$', ''),
                Issuer: self.user
            }

            $.when(commerceService.issueCredits(requestParameters)
            .done(function (response) {
                if (response.Successful) {
                    toastr.success(response.Message, 'Issue Credits', { timeOut: 2000 });
                } else {
                    toastr.error(response.Message, 'Issue Credits', { timeOut: 10000 });
                }
            }).fail(function (error) {
                self.showAlertMessage(error);
            }).always(function () {
                $('#spinner').hide();
                $('#issueCreditsModal').modal('hide');
            }));


            self.resetIssueCreditValues();
        }

        // end issue credits

        self.findCustomer = function () {
            var self = this;

            self.hideAlertMessages();
            $('#spinner').show();
            self.enableComponents(false);

            $.when(lomoUsers.findCustomers(self.customerFilter())
            .done(function (customers) {
                $('#spinner').hide();
                self.enableComponents(true);

                if (customers.length == 0) {
                    self.customer(undefined);
                    $('.alert-custmers-not-found').show();
                }
                else if (customers.length == 1) {
                    self.customer(customers[0]);
                }
                else if (customers.length > 1) {
                    self.foundCustomersTable.updateData(customers);
                    self.foundCustomers = customers;

                    $('#selectCustomerModal').modal('show');
                }
            }).fail(function (error) {
                $('#spinner').hide();
                self.enableComponents(true);
                self.showAlertMessage(error);
            }));
        }

        self.calculateCustomerCredits = function (transactions) {
            var c = 0;
            transactions.map(function (tr) {
                if (tr.credit_status_id == 500) {                     // SettledAsRedeemed
                    if (tr.reimbursement_tender_id == 0) {          // earn 
                        c += tr.discount_amount;
                    }
                    else if (tr.reimbursement_tender_id == 1) {     // burn
                        c -= tr.discount_amount;
                    }
                }
            });

            return c;
        }

        self.getTransactionHistory = function () {
            var self = this;

            var requestParameters = {
                userId: self.customer().global_id,
            };

            self.showSpinnerInTransactionsTab(true);
            self.transactionHistoryTable.clearTable();
            $.when(commerceService.getEarnBurnHistory(requestParameters)
            .done(function (transactions) {
                self.transactionHistoryTable.updateData(transactions);
                self.showSpinnerInTransactionsTab(false);
                var credits = self.calculateCustomerCredits(transactions);
                self.customerCredits("$" + credits.toFixed(2));

            }).fail(function (error) {
                self.showAlertMessage(error);
                self.showSpinnerInTransactionsTab(false);
            }));
        }

        self.getReferrals = function () {
            var self = this;
            var request = { userid: self.customer().global_id };

            self.referralsTable.clearTable();
            self.showSpinnerInReferralsTab(true);
            $.when(commerceService.getReferrals(request)
            .done(function (referrals) {
                self.referralsTable.updateData(referrals);
                self.showSpinnerInReferralsTab(false);
            }).fail(function (error) {
                self.showAlertMessage(error);
                self.showSpinnerInReferralsTab(false);
            }));
        }

        self.getNotes = function () {
            //var self = this;

            //self.showSpinnerInNotesTab(true);
            //var n3 = new noteViewModel({
            //    date: '1 Jan. 2016',
            //    time: '00:00',
            //    agoText: '4 days',
            //    topic: 'Happy New Year!',
            //    text: 'Lorem ipsum'
            //});

            //var newNotesTimeline = new notesTimelineViewModel();
            //newNotesTimeline.addNote(n3);

            //self.notesTimeline(newNotesTimeline);
            //self.showSpinnerInNotesTab(false);
        }

        self.getEarnBurnLineItems = function () {
            var self = this;
            var request = { userid: self.customer().global_id };

            self.earnBurnLineItemsTable.clearTable();
            self.showSpinnerInEarnBurnLineItemsTab(true);
            $.when(commerceService.getEarnBurnLineItems(request)
            .done(function (data) {
                self.earnBurnLineItemsTable.updateData(data);
                self.showSpinnerInEarnBurnLineItemsTab(false);
            }).fail(function (error) {
                self.showAlertMessage(error);
                self.showSpinnerInEarnBurnLineItemsTab(false);
            }));
        }

        self.initUI();

        self.updateUrlInBrowser = function () {
            var newurl = utils.getActionPath() + "?" + jQuery.param({ "customerGlobalId": self.customer().global_id });
            window.history.pushState("Customer Service", "", newurl);
        }

        var urlParameters = $.deparam(window.location.search.substring(1));
        if (urlParameters.customerGlobalId !== undefined) {
            self.customerFilter(urlParameters.customerGlobalId);
            self.findCustomer();
        }
    }
}());