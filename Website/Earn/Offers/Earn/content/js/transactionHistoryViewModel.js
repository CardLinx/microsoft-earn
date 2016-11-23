/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {

    window.transactionHistoryViewModel = function (isHistoryPage) {
        var self = this;
        self.pendingRedemptionHistoryGrid;
        self.pendingRedemptionHistoryGridMobile;
        self.redemptionHistoryGrid;
        self.redemptionHistoryGridMobile;

        self.pendingsTransactionsGridOptions = {
            pageable: false,
            scrollable: false,
            sortable: true
        };

        self.desktopGridOptions = {
            dataSource: {
                pageSize: 50
            },
            pageable: {
                buttonCount: 7,
                messages: {
                    display: "{0} - {1} of {2} transactions",
                    empty: "No transactions to display",
                }
            },
            scrollable: false,
            sortable: true
        };

        self.mobileGridOptions = {
            dataSource: {
                pageSize: 15
            },
            pageable: {
                buttonCount: 1,
                messages: {
                    display: "{0} - {1} of {2} transactions",
                    empty: "No transactions to display",
                }
            },
            scrollable: false,
            sortable: true
        };

        self.calcPagerButtonsCount = function () {
            var w = $($("div.mobile")[0]).width();
            var buttonWidth = 26;
            var totalPagerNavWidth = 4 * buttonWidth;
            var result = (w - totalPagerNavWidth) / buttonWidth - 3;
            return result;
        }

        self.initGrids = function () {

            self.pendingRedemptionHistoryGrid = $("#pendingRedemptionHistoryGrid").kendoGrid(self.pendingsTransactionsGridOptions).data("kendoGrid");
            $("#pendingRedemptionHistoryGrid").show().parents("div.desktop").find(".loading-info").hide();

            self.pendingRedemptionHistoryGridMobile = $("#pendingRedemptionHistoryGridMobile").kendoGrid(self.pendingsTransactionsGridOptions).data("kendoGrid");
            $("#pendingRedemptionHistoryGridMobile").show().parents("div.mobile").find(".loading-info").hide();

            self.redemptionHistoryGrid = $("#redemptionHistoryGrid").kendoGrid(self.desktopGridOptions).data("kendoGrid");
            $("#redemptionHistoryGrid").show().parents("div.desktop").find(".loading-info").hide();

            self.mobileGridOptions.pageable.buttonCount = Math.floor(self.calcPagerButtonsCount());
            self.redemptionHistoryGridMobile = $("#redemptionHistoryGridMobile").kendoGrid(self.mobileGridOptions).data("kendoGrid");
            $("#redemptionHistoryGridMobile").show().parents("div.mobile").find(".loading-info").hide();

            self.copyAndBindTopPager = function (e) {
                var gridSelector = '#' + e.sender.element[0].id;
                var gridView = $(gridSelector).data('kendoGrid'),
                    pager = $('#div .k-pager-wrap'),
                    id = pager.attr('id') + '_top',
                    $grid = $(gridSelector);
                if (gridView.topPager == null) {
                    var topPager = $('<div/>', {
                        id: id,
                        class: 'k-pager-wrap pagerTop'
                    }).insertBefore($grid.find('.k-grid-header'));
                    gridView.topPager = new kendo.ui.Pager(topPager, $.extend({}, gridView.options.pageable, { dataSource: gridView.dataSource }));
                    gridView.options.pagerId = id;
                    gridView.topPager.refresh();
                }
            };

            if (self.redemptionHistoryGridMobile !== undefined) {
                self.redemptionHistoryGridMobile.bind("dataBound", self.copyAndBindTopPager)
                self.redemptionHistoryGridMobile.dataSource.fetch();
            }
        }

        if (isHistoryPage)
        {
            self.initGrids();
        }
    };

}());