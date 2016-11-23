//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Earn.Dashboard.Web.Utils;
using Microsoft.AspNet.SignalR;
using Microsoft.Azure.ActiveDirectory.GraphClient;

namespace Earn.Dashboard.Web.Hubs
{
    public class OnlineUser
    {
        public string Name { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
        public string Photo { get; set; }
    }

    [Authorize]
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, OnlineUser> Users = new ConcurrentDictionary<string, OnlineUser>(StringComparer.InvariantCultureIgnoreCase);

        public void Send(string message)
        {
            OnlineUser sender = GetUser(Context.User.Identity.Name);
            Clients.All.received(new { sender = sender.Name, message = message, isPrivate = false, photo = sender.Photo });
        }

        public void Send(string message, string to)
        {
            OnlineUser receiver;
            if (Users.TryGetValue(to, out receiver))
            {
                OnlineUser sender = GetUser(Context.User.Identity.Name);
                IEnumerable<string> allReceivers;
                lock (receiver.ConnectionIds)
                {
                    lock (sender.ConnectionIds)
                    {
                        allReceivers = receiver.ConnectionIds.Concat(sender.ConnectionIds);
                    }
                }

                foreach (var cid in allReceivers)
                {
                    Clients.Client(cid).received(new { sender = sender.Name, message = message, isPrivate = true, photo = sender.Photo });
                }
            }
        }

        public object GetConnectedUsers()
        {
            //return Users.Where(x =>
            //{
            //    lock (x.Value.ConnectionIds)
            //    {
            //        return !x.Value.ConnectionIds.Contains(Context.ConnectionId, StringComparer.InvariantCultureIgnoreCase);
            //    }
            //}).Select(x => new { name = x.Key, photo = x.Value.Photo });

            return Users.Select(x => new { name = x.Key, photo = x.Value.Photo });
        }

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;
            string photo = null;
            Cookie ThumbnailPhotoCookie;
            if (Context.RequestCookies.TryGetValue(Config.ThumbnailPhotoCookie, out ThumbnailPhotoCookie))
            {
                photo = HttpUtility.UrlDecode(ThumbnailPhotoCookie.Value).Replace(' ', '+');
            }

            var user = Users.GetOrAdd(userName, _ => new OnlineUser
            {
                Name = userName,
                ConnectionIds = new HashSet<string>(),
                Photo = photo
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                // // broadcast this to all clients other than the caller
                // Clients.AllExcept(user.ConnectionIds.ToArray()).userConnected(userName);

                // Or you might want to only broadcast this info if this 
                // is the first connection of the user
                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userName, photo);
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;
            OnlineUser user;
            Users.TryGetValue(userName, out user);
            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        OnlineUser removedUser;
                        Users.TryRemove(userName, out removedUser);
                        Clients.Others.userDisconnected(user.Name, user.Photo);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        private OnlineUser GetUser(string username)
        {
            OnlineUser user;
            Users.TryGetValue(username, out user);
            return user;
        }
    }
}