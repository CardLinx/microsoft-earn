/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {
    window.webAnalyticsInfoViewModel = function (userIdentityName) {
        var self = this;

        var now = new Date();
        self.startDate = ko.observable(now.addDays(-14));
        self.endDate = ko.observable(now);

        // table
        self.resultDataTable

        // chart widgets
        self.byDeviceTypePieChartWidget;
        self.byTopVisitedPagesPieChartWidget;
        self.byVisitorsLineChartWidget;
        self.newUsersBarChartWidget;

        self.initUI = function () {
            // datepicker
            $('#dateRange').daterangepicker({
                locale: {
                    format: 'MM/DD/YYYY'
                },
                startDate: self.startDate(),
                endDate: self.endDate()
            },
            function (start, end, label) {
                self.startDate(start._d);
                self.endDate(end._d);
            });

            // init table
            self.resultDataTable = new window.extendedTableWidget(
                '#resultDataTable',
                window.datatableDefinitions.webAnalyticsColumns,
                1, 5, 'desc', 50, true,
                undefined, undefined, undefined, window.datatableDefinitions.webAnalyticsRowCreateCallback);

            // init chart widgets
            self.byDeviceTypePieChartWidget = new pieChartWidget("#device-types-panel");
            self.byTopVisitedPagesPieChartWidget = new pieChartWidget("#top-visited-pages-panel");

            self.byVisitorsLineChartWidget = new lineChartWidget("#visitors-by-date-panel");
            self.newUsersBarChartWidget = new barChartWidget("#new-users-by-date-panel");

            $('.nav-tabs a[href="#tabs-charts"]').tab('show');
        }

        self.callGetAjax = function (url, parameters, successCallback, errorCallback) {
            $('.alert').hide();
            $.ajax({
                url: url,
                type: "GET",
                data: parameters
            }).success(function (data) {
                successCallback(data);
            }).error(function (errResponse) {
                if (errResponse.status == 403) {
                    $('.alert-warning').show();
                } else {
                    $('.alert-danger')
                        .find('p')
                        .html(errResponse.responseText);

                    $('.alert-danger').show();
                }

                errorCallback();
            });
        }

        // Device Types Pie Chart

        self.drawDeviceTypesPieChart = function (data) {
            var chartData = self.byDeviceTypePieChartWidget.formatPieChartData(data, "device_type", "count");
            self.byDeviceTypePieChartWidget.setData(chartData);
        }

        self.deviceTypesPieChartErrorCallback = function () {
            self.byDeviceTypePieChartWidget.clearChart();
            self.byDeviceTypePieChartWidget.setText("Error");
        }

        // Page Visitors Pie Chart

        self.drawTopVisitedPagesPieChart = function (data) {
            var chartData = self.byTopVisitedPagesPieChartWidget.formatPieChartData(data.slice(0, 9), "page_title", "count");
            self.byTopVisitedPagesPieChartWidget.setData(chartData);
        }

        self.topVisitedPagesPieChartErrorCallback = function () {
            self.byTopVisitedPagesPieChartWidget.clearChart();
            self.byTopVisitedPagesPieChartWidget.setText("Error");
        }

        // Visitors Line Chart

        self.drawVisitorsLineChart = function (data) {
            var chartData = self.byVisitorsLineChartWidget.formatLineChartData(data, "date", "total_visitors");
            self.byVisitorsLineChartWidget.setData(chartData);
        }

        self.visitorsLineChartErrorCallback = function () {
            self.byVisitorsLineChartWidget.clearChart();
            self.byVisitorsLineChartWidget.setText("Error");
        }

        // New Users Bar Chart

        self.drawNewUsersBarChart = function (data) {
            var chartData = self.newUsersBarChartWidget.formatBarChartData(data, "date", "new_users");
            self.newUsersBarChartWidget.setData(chartData);
        }

        self.newUsersBarChartErrorCallback = function (data) {
            self.newUsersBarChartWidget.clearChart();
            self.newUsersBarChartWidget.setText("Error");
        }

        // Data table

        self.fillDataTable = function (data) {
            self.resultDataTable.clearTable();
            self.resultDataTable.updateData(data);
        }

        self.fillDateTableErrorCallback = function (data) {
            self.resultDataTable.clearTable();
        }

        self.getData = function () {
            var parameters = {
                startDate: self.startDate().shortDateToString(),
                endDate: self.endDate().addDays(1).shortDateToString(),
                campaignId: $('#campaignId').val(),
                eventId: $("input[name=eventTypeRadios]:checked").val()
            };

            $('.spinner-loader').show();

            self.byDeviceTypePieChartWidget.clearChart();
            self.byDeviceTypePieChartWidget.setText("Loading...");
            self.callGetAjax("/api/webanalytics/FetchByDeviceTypeAnalyticsAsync", parameters, self.drawDeviceTypesPieChart, self.deviceTypesPieChartErrorCallback);

            self.byTopVisitedPagesPieChartWidget.clearChart();
            self.byTopVisitedPagesPieChartWidget.setText("Loading...");
            self.callGetAjax("/api/webanalytics/FetchByPageTitleAnalyticsAsync", parameters, self.drawTopVisitedPagesPieChart, self.topVisitedPagesPieChartErrorCallback);
            
            self.byVisitorsLineChartWidget.clearChart();
            self.byVisitorsLineChartWidget.setText("Loading...");
            self.callGetAjax("/api/webanalytics/FetchVisitorsAnalyticsAsync", parameters, self.drawVisitorsLineChart, self.visitorsLineChartErrorCallback);

            self.newUsersBarChartWidget.clearChart();
            self.newUsersBarChartWidget.setText("Loading...");
            self.callGetAjax("/api/webanalytics/FetchNewUsersAnalyticsAsync", parameters, self.drawNewUsersBarChart, self.newUsersBarChartErrorCallback);

            self.callGetAjax("/api/webanalytics/FetchAnalyticsAsync", parameters, self.fillDataTable, self.newUsersBarChartErrorCallback);
        }

        self.initUI();
    }
}());