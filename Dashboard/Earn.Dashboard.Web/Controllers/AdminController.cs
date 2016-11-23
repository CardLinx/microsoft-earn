//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Earn.Dashboard.Web.Attributes;
using Earn.Dashboard.Web.Utils;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;

namespace Earn.Dashboard.Web.Controllers
{
    [AuthorizeSG(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("adduser");
        }

        public async Task<ActionResult> AddUser()
        {
            ActiveDirectoryClient activeDirectoryClient = GraphHelper.ActiveDirectoryClient();
            Dictionary<string, IEnumerable<string>> model = new Dictionary<string, IEnumerable<string>>();
            List<string> users = null;
            var groups = await activeDirectoryClient.Groups.Where(group => group.ObjectId.Equals(Config.SecurityGroups[Config.Roles.User])).ExecuteAsync();
            IGroupFetcher retrievedGroupFetcher = groups.CurrentPage.FirstOrDefault() as Group;
            if (retrievedGroupFetcher != null)
            {
                users = new List<string>();
                IPagedCollection<IDirectoryObject> members = await retrievedGroupFetcher.Members.ExecuteAsync();
                do
                {
                    List<IDirectoryObject> directoryObjects = members.CurrentPage.ToList();
                    foreach (IDirectoryObject member in directoryObjects)
                    {
                        if (member is User)
                        {
                            User user = member as User;
                            users.Add(string.Format("{0}, ({1})", user.DisplayName, user.UserPrincipalName));
                        }
                        else if (member is Group)
                        {
                            Group group = member as Group;
                            users.Add(string.Format("{0}, (Group)", group.DisplayName));
                        }
                        else if (member is Contact)
                        {
                            Contact contact = member as Contact;
                            users.Add(string.Format("{0}, (Contact)", contact.DisplayName));
                        }
                    }
                    members = members.GetNextPageAsync().Result;
                } while (members != null);
            }

            model.Add(Config.Roles.User, users);
            users = null;
            groups = await activeDirectoryClient.Groups.Where(group => group.ObjectId.Equals(Config.SecurityGroups[Config.Roles.Admin])).ExecuteAsync();
            retrievedGroupFetcher = groups.CurrentPage.FirstOrDefault() as Group;
            users = new List<string>();
            if (retrievedGroupFetcher != null)
            {
                users = new List<string>();
                IPagedCollection<IDirectoryObject> members = await retrievedGroupFetcher.Members.ExecuteAsync();
                do
                {
                    List<IDirectoryObject> directoryObjects = members.CurrentPage.ToList();
                    foreach (IDirectoryObject member in directoryObjects)
                    {
                        if (member is User)
                        {
                            User user = member as User;
                            users.Add(string.Format("{0}, ({1})", user.DisplayName, user.UserPrincipalName));
                        }
                        if (member is Group)
                        {
                            Group group = member as Group;
                            users.Add(string.Format("Group - {0}", group.DisplayName));
                        }
                        if (member is Contact)
                        {
                            Contact contact = member as Contact;
                            users.Add(string.Format("Contact - {0}", contact.DisplayName));
                        }
                    }
                    members = members.GetNextPageAsync().Result;
                } while (members != null);
            }

            model.Add(Config.Roles.Admin, users);

            users = null;
            groups = await activeDirectoryClient.Groups.Where(group => group.ObjectId.Equals(Config.SecurityGroups[Config.Roles.Support])).ExecuteAsync();
            retrievedGroupFetcher = groups.CurrentPage.FirstOrDefault() as Group;
            users = new List<string>();
            if (retrievedGroupFetcher != null)
            {
                users = new List<string>();
                IPagedCollection<IDirectoryObject> members = await retrievedGroupFetcher.Members.ExecuteAsync();
                do
                {
                    List<IDirectoryObject> directoryObjects = members.CurrentPage.ToList();
                    foreach (IDirectoryObject member in directoryObjects)
                    {
                        if (member is User)
                        {
                            User user = member as User;
                            users.Add(string.Format("{0}, ({1})", user.DisplayName, user.UserPrincipalName));
                        }
                        if (member is Group)
                        {
                            Group group = member as Group;
                            users.Add(string.Format("Group - {0}", group.DisplayName));
                        }
                        if (member is Contact)
                        {
                            Contact contact = member as Contact;
                            users.Add(string.Format("Contact - {0}", contact.DisplayName));
                        }
                    }
                    members = members.GetNextPageAsync().Result;
                } while (members != null);
            }

            model.Add(Config.Roles.Support, users);
            return View(model);
        }

        public ActionResult Notify()
        {
            return View();
        }
    }
}