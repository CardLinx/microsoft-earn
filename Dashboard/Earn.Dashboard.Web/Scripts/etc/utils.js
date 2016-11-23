/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/*global    window,document*/
/*
    common stuff.
*/

(function () {
    "use strict";
    window.utils = {
        generateRandomColor: function () {
            var letters = '0123456789ABCDEF'.split('');
            var color = '#';
            for (var i = 0; i < 6; i++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            return color;
        },

        getActionPath: function() {
            return window.location.protocol + "//" + window.location.host + window.location.pathname;
        },

        getCardLogo: function (data) {
            var basePath = "/Content/img/";
            var logoFileName = "";
            
            if (data == 3) {
                logoFileName = "amex.png";
            }
            else if (data == 4) {
                logoFileName = "visa.png";
            }
            else if (data == 5) {
                logoFileName = "mc.png";
            }

            return basePath + logoFileName;
        }
    };

    Date.prototype.addDays = function (days) {
        var dat = new Date(this.valueOf());
        dat.setDate(dat.getDate() + days);
        return dat;
    }

    Date.prototype.getDayOfWeek = function () {
        var weekday = new Array(7);
        weekday[0] = "Sun";
        weekday[1] = "Mon";
        weekday[2] = "Tue";
        weekday[3] = "Wed";
        weekday[4] = "Thu";
        weekday[5] = "Fri";
        weekday[6] = "Sat";

        var n = weekday[this.getDay()];
        return n;
    }

    // returns "MM/DD (weekday)"
    Date.prototype.shortDateToString = function () {
        var mm = (this.getMonth() + 1).toString(); // getMonth() is zero-based
        var dd = this.getDate().toString();
        return (mm[1] ? mm : "0" + mm[0]) + "/" + (dd[1] ? dd : "0" + dd[0]) + " (" + this.getDayOfWeek() + ")"; // padding
    }

    // returns "MM/DD/YYYY"
    Date.prototype.shortDateToString = function () {
        var yyyy = this.getFullYear().toString();
        var mm = (this.getMonth() + 1).toString(); // getMonth() is zero-based
        var dd = this.getDate().toString();
        return (mm[1] ? mm : "0" + mm[0]) + "/" + (dd[1] ? dd : "0" + dd[0]) + "/" + yyyy;
    }

    // returns "MM/DD/YYYY HH:mm:SS"
    Date.prototype.fullDateToString = function () {
        var hh = this.getHours().toString();
        var mm = this.getMinutes().toString();
        var ss = this.getSeconds().toString();
        return this.shortDateToString() + " " + (hh[1] ? hh : "0" + hh ) + ":" + (mm[1] ? mm : "0" + mm ) + ":" + (ss[1] ? ss : "0" + ss) ;
    }

    Date.prototype.expirationDateFormat = function () {
        var yy = this.getFullYear().toString().substring(2, 4);
        var mm = (this.getMonth() + 1).toString(); // getMonth() is zero-based

        return yy + '/' + mm;
    }

}());