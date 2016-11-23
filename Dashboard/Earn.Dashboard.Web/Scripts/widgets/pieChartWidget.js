/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {
    window.pieChartWidget = function (panelSelector, options) {

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

        self.formatPieChartData = function (data, keyField, valueField) {
            var result = [];
            for (var i = 0; i < data.length; i++) {
                var randomColor = utils.generateRandomColor();
                result.push(
                    {
                        label: data[i][keyField],
                        value: data[i][valueField],
                        color: randomColor,
                        highlight: randomColor
                    })
            }

            return result;
        }

        self.defaultChartOptions = {
            //Boolean - Whether we should show a stroke on each segment
            segmentShowStroke: true,
            //String - The colour of each segment stroke
            segmentStrokeColor: "#fff",
            //Number - The width of each segment stroke
            segmentStrokeWidth: 1,
            //Number - Amount of animation steps
            animationSteps: 50,
            //String - Animation easing effect
            animationEasing: "linear",
            //Boolean - Whether we animate scaling the Doughnut from the centre
            animateScale: true,
            //Boolean - whether to make the chart responsive to window resizing
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: false,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\">" +
                                 "<% for (var i=0; i<segments.length; i++){%>" +
                                     "<li>" +
                                         "<i class=\"fa fa-circle-o\" style=\"color:<%=segments[i].fillColor%>\"></i>" +
                                         "<%if(segments[i].label){%><%=segments[i].label%> (<%=segments[i].value%>)<%}%>" +
                                     "</li>" +
                                 "<%}%>" +
                            "</ul>",
            //String - A tooltip template
            tooltipTemplate: "<%=value %> <%=label%>",
        };

        self.chartOptions = options === undefined
                                        ? self.defaultChartOptions
                                        : options;
        self.panel = $(panelSelector);

        self.chartCanvas = self.panel.find(".chart-canvas-container canvas").get(0).getContext("2d");

        self.chart;

        self.setData = function (data) {
            if (self.chart === undefined) {
                self.chart = new Chart(self.chartCanvas).Doughnut(data, self.chartOptions);
            } else {
                self.clearChart();
                self.updateData(data);
            }

            self.AppendNewLegend();

            if (data.length == 0) {
                self.setText("No Data");
            } else {
                self.hideText();
            }
        }

        self.updateData = function (newData) {
            for (var i = 0; i < newData.length; i++) {
                self.chart.addData(newData[i]);
            }
        }

        self.clearChart = function () {
            if (self.chart === undefined) {
                return;
            }

            self.clearData();
            self.chart.clear();
            self.clearLegend();
        }

        self.clearData = function () {
            while (self.chart.segments.length > 0) {
                self.chart.removeData();
            }
        }

        self.clearLegend = function () {
            var legend = self.chart.generateLegend()
            $(self.panel.find('.chart-legend-container')[0]).empty();
        }

        self.AppendNewLegend = function () {
            var legend = self.chart.generateLegend()
            $(self.panel.find('.chart-legend-container')[0]).append(legend);
        }
    }

})