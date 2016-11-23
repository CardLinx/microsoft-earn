/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {
    window.barChartWidget = function (panelSelector, data, options) {

        var self = this;

        self.showText = function () {
            $(self.panel.find('.chart-text')[0]).show();
        }

        self.hideText = function () {
            $(self.panel.find('.chart-text')[0]).hide();
        }

        self.setText = function (newText) {
            $(self.panel.find('.chart-text')[0]).text(newText);
            self.showText();
        }

        self.formatBarChartData = function (data, keyField, valueField) {
            var days = [];
            var total = [];
            var newUsers = [];

            var result;
            for (var i = 0; i < data.length; i++) {
                var d = new Date(data[i][keyField])
                days.push(d.shortDateToString());
                total.push(data[i][valueField] !== undefined ? data[i][valueField] : 0);
            }

            result = {
                labels: days,
                datasets: [
                    {
                        label: "Total Visitors",
                        fillColor: "rgba(151,187,205,0.5)",
                        strokeColor: "rgba(151,187,205,0.8)",
                        highlightFill: "rgba(151,187,205,0.75)",
                        highlightStroke: "rgba(151,187,205,1)",
                        pointColor: "rgba(151,187,205,1)",
                        pointStrokeColor: "#fff",
                        pointHighlightFill: "#fff",
                        pointHighlightStroke: "rgba(151,187,205,1)",
                        data: total
                    }
                ]
            };

            return result;
        }

        self.defaultChartOptions = {

            animation: false,

            //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
            scaleBeginAtZero: true,

            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: true,

            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",

            //Number - Width of the grid lines
            scaleGridLineWidth: 1,

            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,

            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,

            //Boolean - If there is a stroke on each bar
            barShowStroke: true,

            //Number - Pixel width of the bar stroke
            barStrokeWidth: 2,

            //Number - Spacing between each of the X value sets
            barValueSpacing: 5,

            //Number - Spacing between data sets within X values
            barDatasetSpacing: 1,

            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",

            // Boolean - whether or not the chart should be responsive and resize when the browser does.
            responsive: true,

            showTooltips: true
        };

        self.chartOptions = options === undefined
                                        ? self.defaultChartOptions
                                        : options;
        self.panel = $(panelSelector);
        self.chartCanvas = self.panel.find(".chart-canvas-container canvas").get(0).getContext("2d");

        self.chart;

        self.setData = function (data) {
            if (self.chart === undefined) {
                self.chart = new Chart(self.chartCanvas).Bar(data, self.chartOptions);
            } else {
                self.clearData();
                self.updateData(data);
            }

            if (data.datasets[0].data.length == 0) {
                self.setText("No Data");
            } else {
                self.hideText();
            }
        }

        self.updateData = function (newData) {
            for (var i = 0; i < newData.labels.length; i++) {
                self.chart.addData([newData.datasets[0].data[i]], newData.labels[i]);
            }
        }

        self.clearChart = function () {
            if (self.chart === undefined) {
                return;
            }

            self.clearData();
            self.chart.clear();
        }

        self.clearData = function () {
            while (self.chart.datasets[0].bars.length > 0) {
                self.chart.removeData();
            }
        }
    }
})