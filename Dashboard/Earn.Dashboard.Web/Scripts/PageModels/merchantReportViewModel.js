/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {

    window.requestParameters = function (urlParameters) {
        var self = this;
        var now = new Date();
        self.startDate = now.addDays(-7);
        self.endDate = now;

        self.requestType = urlParameters.requestType ? Number(urlParameters.requestType) : 1;
        self.fetchData = urlParameters.fetchData ? urlParameters.fetchData === 'true' : false;
        self.startDate = urlParameters.startDate ? urlParameters.startDate : self.startDate.shortDateToString() + " 00:00:00";
        self.endDate = urlParameters.endDate ? urlParameters.endDate : self.endDate.shortDateToString() + " 23:59:59";
        self.transactionTypes = urlParameters.transactionTypes ? urlParameters.transactionTypes.map(Number) : [0, 1];
        self.cardBrandIds = urlParameters.cardBrandIds ? urlParameters.cardBrandIds.map(Number) : [3, 4, 5, 6];
        self.last4Digits = urlParameters.last4Digits ? urlParameters.last4Digits : "";
        self.merchantName = ko.observable(urlParameters.merchantName ? urlParameters.merchantName  : "All");
        self.sortColumnIndex = urlParameters.sortColumnIndex ? Number(urlParameters.sortColumnIndex) : undefined;
        self.sortDirection = urlParameters.sortDirection ?  urlParameters.sortDirection : "desc";
        self.rowsInPage = urlParameters.rowsInPage ? Number(urlParameters.rowsInPage) : 100;
        self.pageN = urlParameters.pageN ? Number(urlParameters.pageN) : 1;
    }

    window.merchantReportViewModel = function (serverPageModel)
    {
        var self = this;

        self.getSortOptionsFromRequestParameters = function () {
            if (self.requestParameters.requestType == 1) {
                return (self.requestParameters.sortColumnIndex
                    ? [self.requestParameters.sortColumnIndex, self.requestParameters.sortDirection]
                    : [self.defaultTransactionsSortColumn, "desc"]);
            } else if (self.requestParameters.requestType == 2) {
                return (self.requestParameters.sortColumnIndex
                    ? [self.requestParameters.sortColumnIndex, self.requestParameters.sortDirection]
                    : [self.defaultSettlementsSortColumn, "desc"]);
            }
        }

        self.getSortOptionsBasedOnRequestType = function () {
            if ((self.prevRequestType != self.requestParameters.requestType) && (self.requestParameters.requestType == 1)) {
                return [self.defaultTransactionsSortColumn, "desc"];
            } else if ((self.prevRequestType != self.requestParameters.requestType) && (self.requestParameters.requestType == 2)) {
                return [self.defaultSettlementsSortColumn, "desc"];
            } else {
                return self.getSortOptionsFromRequestParameters();
            }
        }

        self.requestTypes = [
            { id: 1, name: "Transactions History" },
            { id: 2, name: "Settlements" }
        ]

        self.defaultTransactionsSortColumn = 1;
        self.defaultSettlementsSortColumn = 7;

        var urlParameters = $.deparam(window.location.search.substring(1));
        self.requestParameters = new requestParameters(urlParameters);

        var sortOptions = self.getSortOptionsFromRequestParameters();
        self.requestParameters.sortColumnIndex = sortOptions[0];
        self.requestParameters.sortDirection = sortOptions[1];

        self.prevRequestType = self.requestParameters.requestType;

        self.merchants = ko.observable(serverPageModel.Merchants);
        self.cardBrands = ko.observable(serverPageModel.CardBrands);

        self.transactionTypes = ko.observable(serverPageModel.TransactionTypes)

        self.activeTable;
        self.transactionsHistoryTable;
        self.settlementsTable;

        self.updateUrlInBrowser = function () {
            var newurl = utils.getActionPath() + "?" + jQuery.param(self.requestParameters)
            window.history.pushState("Transactions", "", newurl);
        }

        self.showErrorMessage = function (error) {
            if (error.status == 403) {
                $('.alert-warning').show();
            } else {
                $('.alert-danger')
                    .find('p')
                    .html(error.responseText);

                $('.alert-danger').show();
            }
        }

        self.getData = function () {
            var self = this;
            var sortOptions = self.getSortOptionsBasedOnRequestType();

            self.requestParameters.fetchData = true;
            self.requestParameters.sortColumnIndex = sortOptions[0];
            self.requestParameters.sortDirection = sortOptions[1];

            self.updateUrlInBrowser();

            var api;
            if (self.requestParameters.requestType == 1) {
                api = commerceService.getTransactions;
                self.activeTable = self.transactionsHistoryTable;
            } else if (self.requestParameters.requestType == 2) {
                api = commerceService.getSettlements;
                self.activeTable = self.settlementsTable;
            }



            $('.alert').hide();
            $('#spinner').show();

            $.when(api(self.requestParameters)
            .done(function (data) {
                $('#spinner').hide();
                if (self.requestParameters.requestType == 1) {
                    self.transactionsHistoryTable.show();
                    self.settlementsTable.hide();
                } else if (self.requestParameters.requestType == 2) {
                    self.settlementsTable.show();
                    self.transactionsHistoryTable.hide();
                }

                self.activeTable.setSortColumnIndex(sortOptions[0], sortOptions[1]);
                self.activeTable.updateData(data);
                self.prevRequestType = self.requestParameters.requestType;
            }).fail(function (error) {
                $('#spinner').hide();
                self.showErrorMessage(error);
            }));
        }

        self.onOrder = function () {
            self.requestParameters.sortColumnIndex = self.activeTable.sortColumnIndex;
            self.requestParameters.sortDirection = self.activeTable.sortDirection;
            if (self.requestParameters.fetchData) {
                self.updateUrlInBrowser();
            }
        }

        self.onPage = function () {
            self.requestParameters.pageN = self.activeTable.pageN;
            if (self.requestParameters.fetchData) {
                self.updateUrlInBrowser();
            }
        }

        self.onLength = function () {
            self.requestParameters.rowsInPage = self.activeTable.pageLength;
            if (self.requestParameters.fetchData) {
                self.updateUrlInBrowser();
            }
        }

        self.initUI = function () {
            // datepicker
            $('#dateRange').daterangepicker({
                locale: {
                    format: 'MM/DD/YYYY'
                },
                startDate: self.requestParameters.startDate,
                endDate: self.requestParameters.endDate
            },
            function (start, end, label) {
                self.requestParameters.startDate = start._d.fullDateToString();
                self.requestParameters.endDate = end._d.fullDateToString();
            });


            var transactionsSortColumnIndex = (self.requestParameters.requestType == 1) && (self.requestParameters.sortColumnIndex) ? self.requestParameters.sortColumnIndex : self.defaultTransactionsSortColumn;
            // table
            self.transactionsHistoryTable = new extendedTableWidget(
                '#transactionsDataTable',
                datatableDefinitions.transactionsHistoryColumns,
                self.requestParameters.pageN, transactionsSortColumnIndex, self.requestParameters.sortDirection, self.requestParameters.rowsInPage, true,
                self.onOrder, self.onPage, self.onLength, datatableDefinitions.transactionsHistoryRowCreateCallback, datatableDefinitions.transactionsHistoryRowCreatedCallback
            );

            if (self.requestParameters.requestType != 1) {
                self.transactionsHistoryTable.hide();
            }

            var settlementsSortColumnIndex = (self.requestParameters.requestType == 2) && (self.requestParameters.sortColumnIndex) ? self.requestParameters.sortColumnIndex : self.defaultSettlementsSortColumn;
            self.settlementsTable = new extendedTableWidget(
                '#settlementsDataTable',
                datatableDefinitions.settlementsColumns,
                self.requestParameters.pageN, settlementsSortColumnIndex, self.requestParameters.sortDirection, self.requestParameters.rowsInPage, true,
                self.onOrder, self.onPage, self.onLength, datatableDefinitions.settlementsRowCreateCallback, datatableDefinitions.settlementsRowCreatedCallback
            );

            if (self.requestParameters.requestType != 2) {
                self.settlementsTable.hide();
            }
        }

        self.initUI();

        if (self.requestParameters.fetchData)
        {
            self.getData();
        }
    }
}());