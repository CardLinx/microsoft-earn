/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {
    window.extendedTableWidget = function (
        elemSelector,
        columns, 
        initialPageN, initialSortColumnIndex, initialSortDirection, initialPageLength, 
        showExportButtons,
        orderChangedCallback, pageChangedCallback, lengthChangedCallback, createRowCallback, createdRowCallback) {

        var self = this;
        self.selector = elemSelector;
        self.table;
        self.pageN = initialPageN;

        self.sortColumnIndex = initialSortColumnIndex;
        self.sortDirection = initialSortDirection;

        self.pageLength = initialPageLength;

        self.onOrder = function () {
            var t = $(self.selector).dataTable();
            self.sortColumnIndex = t.fnSettings().aaSorting[0][0];
            self.sortDirection = t.fnSettings().aaSorting[0][1];

            if (self.table !== undefined && orderChangedCallback !== undefined) {
                orderChangedCallback();
            }
        }

        self.onPage = function () {
            self.pageN = self.table.page.info().page + 1; // jquery datatable page indexes starts from 0

            if (self.table !== undefined && pageChangedCallback !== undefined) {
                pageChangedCallback();
            }
        }

        self.onLength = function () {
            self.pageLength = self.table.page.info().length;

            if (self.table !== undefined && lengthChangedCallback !== undefined) {
                lengthChangedCallback();
            }
        }

        var tableSettings = {
            "autoWidth": false,
            "pageLength": self.pageLength,
            "lengthMenu": [
                        [25, 50, 100, -1],
                        ['25 rows', '50 rows', '100 rows', 'Show all']
            ],
            "order": [[self.sortColumnIndex, self.sortDirection]],
            "columns": columns,
            "createdRow": createdRowCallback
        };

        if (showExportButtons) {
            tableSettings["buttons"] = [
                'pageLength',
                {
                    extend: 'copyHtml5',
                    exportOptions: {
                        columns: ':visible',
                        orthogonal: 'export'
                    }
                },
                {
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: ':visible',
                        orthogonal: 'export'
                    }
                },
                {
                    extend: 'csvHtml5',
                    exportOptions: {
                        columns: ':visible',
                        orthogonal: 'export'
                    }
                }
            ];

            tableSettings["dom"] = 'Bfrtip';
        }


        self.table = $(self.selector)
            .on('order.dt', function () { self.onOrder(); })
            .on('page.dt', function () { self.onPage(); })
            .on('length.dt', function () { self.onLength(); })
            .DataTable(tableSettings);

        self.clearTable = function ()
        {
            var t = $(elemSelector).dataTable();
            t.fnClearTable(this);
            t.fnDraw();
        }

        self.updateData = function (newTableData) {
            var arrData = [];

            for (var i = 0; i < newTableData.length; i++)
            {
                arrData.push(createRowCallback(newTableData[i]));
            }

            if (self.table.data().length != 0)
            {
                self.clearTable();
            }

            var t = $(elemSelector).dataTable();
            oSettings = t.fnSettings();

            for (var i = 0; i < arrData.length; i++) {
                t.oApi._fnAddData(oSettings, arrData[i]);
            }

            oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
            t.fnSort([[self.sortColumnIndex, self.sortDirection]]);
            t.fnDraw();

            if (self.pageN > 1) {
                self.table.page(self.pageN - 1).draw('page');
            }
        }

        self.show = function () {
            $(self.selector + '_wrapper').show();
        }

        self.hide = function () {
            $(self.selector + '_wrapper').hide();
        }

        self.setSortColumnIndex = function (index, direction) {
            self.sortColumnIndex = index;
            self.sortDirection = direction;
            var t = $(self.selector).dataTable();
            t.fnSettings().aaSorting[0][0] = self.sortColumnIndex;
            t.fnSettings().aaSorting[0][1] = self.sortDirection;
        }
    }
})