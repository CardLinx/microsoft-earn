/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {

    window.noteViewModel = function (data) {
        var self = this;

        self.date = ko.observable(data.date);
        self.time = ko.observable(data.time);
        self.agoText = ko.observable(data.agoText);

        self.topic = ko.observable(data.topic);
        self.text = ko.observable(data.text);
    }

    window.timelineItemViewModel = function (date) {
        var self = this;

        self.date = ko.observable(date);
        self.notes = ko.observable([]);
    }

    window.notesTimelineViewModel = function () {
        var self = this;

        self.items = ko.observable([]);
        self.totalNotesCount = ko.observable(0);

        self.addNote = function (note) {
            var dateExists = false;

            for (var i = 0; i < self.items().length; i++) {
                if (self.items()[i].date() == note.date()) {
                    self.items()[i].notes().push(note);
                    self.totalNotesCount(self.totalNotesCount() + 1);
                    dateExists = true;
                    break;
                }
            }

            if (!dateExists) {
                var newItem = new timelineItemViewModel(note.date());
                newItem.notes().push(note);
                self.items().push(newItem);
                self.totalNotesCount(self.totalNotesCount() + 1);
            }
        }
    }
}());