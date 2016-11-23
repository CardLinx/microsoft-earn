//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Dal;

namespace UserUpdater
{
    public class UserUpdater
    {
        private static readonly Partitioner Partitioner = new Partitioner();
        
        #region Public Members

        public void DeleteUserByEmail(string email)
        {
            DeleteUserByExternalId(email, 1);
            
        }

        public void DeleteUserByMsId(string msId)
        {
            DeleteUserByExternalId(msId, 0);
        }

        #endregion

        #region Private Members

        private void DeleteUserByExternalId(string id, int type)
        {
            id = id.ToLowerInvariant();
            // TODO change this when moving to multi partitions model
            ExecuteOnFederationMember("0", (context, partitionId) =>{ 
                    var user = context.ExternalUserDbs.SingleOrDefault(_ => _.ExternalId == id && _.AuthProvider == type);
                    if (user != null)
                    {
                        Guid userId = user.UserId;
                        DeleteExternalUserTableRecords(userId, context);
                        DeleteEmailSubscriptions(userId, context);
                        DeleteEmailUnsubscribeUrl(userId, context);
                        DeleteConfirmationCodes(userId, context);
                        DeleteUserTableRecords(userId, context);
                    }
                });
            
            
        }

        private void DeleteEmailSubscriptions(Guid userId, UsersEntities context)
        {
            var emailSubs = context.EmailSubscriptionDbs.Where(elem => elem.UserId == userId);
            foreach (var sub in emailSubs)
            {
                context.EmailSubscriptionDbs.Remove(sub);

            }
            context.SaveChanges();
            var subCount = context.EmailSubscriptionDbs.Count(elem => elem.UserId == userId);
            if (subCount > 0)
            {
                throw new ApplicationException(string.Format("User with Id: {0} was not deleted from table", userId));
            }
            
        }

        private void DeleteEmailUnsubscribeUrl(Guid userId, UsersEntities context)
        {
            var emailUnsubs = context.EmailUnsubscribeUrlDbs.Where(elem => elem.UserId == userId);
            foreach (var unsub in emailUnsubs)
            {
                context.EmailUnsubscribeUrlDbs.Remove(unsub);

            }
            context.SaveChanges();
            var unsubCount = context.EmailUnsubscribeUrlDbs.Count(elem => elem.UserId == userId);
            if (unsubCount > 0)
            {
                throw new ApplicationException(string.Format("User with Id: {0} was not deleted from table", userId));
            }
            
        }

        private void DeleteConfirmationCodes(Guid userId, UsersEntities context)
        {
            var confirmationCodes = context.ConfirmationCodeDbs.Where(elem => elem.UserId == userId);
            foreach (var confirmationCode in confirmationCodes)
            {
                context.ConfirmationCodeDbs.Remove(confirmationCode);

            }
            context.SaveChanges();
            var confCount = context.ConfirmationCodeDbs.Count(elem => elem.UserId == userId);
            if (confCount > 0)
            {
                throw new ApplicationException(string.Format("User with Id: {0} was not deleted from table", userId));
            }
        }

        private void DeleteExternalUserTableRecords(Guid userId, UsersEntities context)
        {
            var records = context.ExternalUserDbs.Where(elem => elem.UserId == userId);
            foreach (var record in records)
            {
                context.ExternalUserDbs.Remove(record);
            }
            context.SaveChanges();
            var count = context.ExternalUserDbs.Count(elem => elem.UserId == userId);
            if (count > 0)
            {
                throw new ApplicationException(string.Format("User with Id: {0} was not deleted from table", userId));
            }
        }

        private void DeleteUserTableRecords(Guid userId, UsersEntities context)
        {
            var user = context.UserDbs.SingleOrDefault(elem => elem.Id == userId);
            if (user != null)
            {
                context.UserDbs.Remove(user);
                context.SaveChanges();
                user = context.UserDbs.SingleOrDefault(elem => elem.Id == userId);
                if (user != null)
                {
                    throw new ApplicationException(string.Format("User with Id: {0} was not deleted from table", userId));
                }
            }
        }

        private static void ExecuteOnFederationMember(string id, Action<UsersEntities, int> action)
        {
            try
            {
                using (var context = new UsersEntities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.Connection.Open();

                    var partitionId = Partitioner.PartitionId(id);
                    //Partitioner.SwitchConnectionToUsersFederationMember(context, partitionId);
                    action(context, partitionId);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        #endregion
    }
}