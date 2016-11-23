/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {
    window.tableWidget = function (
        elemSelector,
        columns,
        createRowCallback,
        createdRowCallback) {

        var self = this;
        self.selector = elemSelector;

        self.table;
        self.selectedData;

        self.table = $(self.selector)
            .DataTable({
                "autoWidth": false,
                "bFilter": false,
                "bInfo" : false,
                "bPaginate": false,
                "ordering": false,
                "columns": columns,
                "createdRow": createdRowCallback
            });

        $(self.selector + ' tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            }
            else {
                self.table.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }

            self.selectedData = self.table.row(this).data();
        });

        self.updateData = function (newTableData) {
            var arrData = [];

            for (var i = 0; i < newTableData.length; i++) {
                arrData.push(createRowCallback(newTableData[i]));
            }

            var t = $(elemSelector).dataTable();
            oSettings = t.fnSettings();

            t.fnClearTable(this);

            for (var i = 0; i < arrData.length; i++) {
                t.oApi._fnAddData(oSettings, arrData[i]);
            }

            oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
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
    }
})