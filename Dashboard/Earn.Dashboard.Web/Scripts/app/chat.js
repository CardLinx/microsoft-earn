/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
/// <reference path="../knockout-2.2.0.debug.js" />
/// <reference path="../jquery-1.8.3.intellisense.js" />
/// <reference path="../jquery.signalR-1.0.0-rc1.js" />

(function () {

    function Message(from, msg, isPrivate, photo) {
        this.from = ko.observable(from);
        this.message = ko.observable(msg);
        this.isPrivate = ko.observable(isPrivate);
        var userPhoto = photo ? photo : "~/Content/img/user.png";
        this.photo = ko.observable(userPhoto);
    }

    function User(name, photo) {
        var self = this;
        self.name = ko.observable(name);
        self.photo = ko.observable(photo);
        self.isPrivateChatUser = ko.observable(false);
        self.setAsPrivateChat = function (user) {
            viewModel.privateChatUser(user.name());
            viewModel.isInPrivateChat(true);
            $.each(viewModel.users(), function (i, user) {
                user.isPrivateChatUser(false);
            });
            self.isPrivateChatUser(true);
        };
    }

    var viewModel = {
        messages: ko.observableArray([]),
        users: ko.observableArray([]),
        isInPrivateChat: ko.observable(false),
        privateChatUser: ko.observable(),
        exitFromPrivateChat: function () {
            viewModel.isInPrivateChat(false);
            viewModel.privateChatUser(null);
            $.each(viewModel.users(), function (i, user) {
                user.isPrivateChatUser(false);
            });
        }
    };

    $(function () {
        var chatHub = $.connection.chatHub,
            $notifyButton = $('#confirmNotify'),
            $messageText = $('#message');

        //$.connection.hub.logging = true;
        chatHub.client.received = function (msg) {
            viewModel.messages.unshift(new Message(msg.sender, msg.message, msg.isPrivate, msg.photo));
            toastr.info(msg.message, msg.sender, { timeOut: 3000 });
        };

        chatHub.client.userConnected = function (name, photo) {
            viewModel.users.unshift(new User(name, photo));
        };

        chatHub.client.userDisconnected = function (name, photo) {
            viewModel.users.pop(new User(name, photo));
            if (viewModel.isInPrivateChat() && viewModel.privateChatUser() === name) {
                viewModel.isInPrivateChat(false);
                viewModel.privateChatUser(null);
            }
        };

        startConnection();
        ko.applyBindings(viewModel, document.getElementById("mainHeader"));

        function startConnection() {
            $.connection.hub.start().done(function () {
                $notifyButton.click(function (e) {
                    sendMessage();
                    e.preventDefault();
                });

                $messageText.focus();

                chatHub.server.getConnectedUsers().done(function (users) {
                    $.each(users, function (i, user) {
                        viewModel.users.push(new User(user.name, user.photo));
                    });
                });
            }).fail(function (err) {
                console.log(err);
            });
        }

        function sendMessage() {
            var msg = $.trim($messageText.val());
            if (viewModel.isInPrivateChat()) {
                chatHub.server.send(msg, viewModel.privateChatUser()).fail(function (err) {
                    console.log('Send method failed: ' + err);
                });
            }
            else {
                chatHub.server.send(msg).fail(function (err) {
                    console.log('Send method failed: ' + err);
                });
            }

            $messageText.val(null);
            $messageText.focus();
        }
    });
}());