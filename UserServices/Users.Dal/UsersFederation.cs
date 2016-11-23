//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Users.Dal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using Lomo.Logging;

    using Microsoft.Practices.TransientFaultHandling;

    /// <summary>
    ///     This class provides fresh metadata for Users database federation. Used by Dal.
    /// </summary>
    internal class UsersFederation
    {
        #region Consts/ Static

        /// <summary>
        ///     FederationName of database federation
        /// </summary>
        private const string FederationName = "Users";

        /// <summary>
        ///     Renew period for timer
        /// </summary>
        private static readonly TimeSpan RenewPeriod = TimeSpan.FromSeconds(30);

        /// <summary>
        ///     Max delay in refresh (seconds)
        /// </summary>
        private static readonly TimeSpan MaxStalePeriod = TimeSpan.FromSeconds(600);

        #endregion

        #region Data Members

        /// <summary>
        ///     The connection string to the users database. If null the default one will be used
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The retry policy.
        /// </summary>
        private readonly RetryPolicy retryPolicy;

        /// <summary>
        ///     The partitions locker.
        /// </summary>
        private readonly object partitionsLocker = new object();

        /// <summary>
        ///     Time when last refresh ran
        /// </summary>
        private DateTime lastRefreshedTime = DateTime.UtcNow;

        /// <summary>
        ///     Low boundary values for each partition (federation member)
        /// </summary>
        private List<int> partitionLowValues;

        /// <summary>
        ///     Timer to renew fedaretion info. We must have reference to this object, otherwise it will be taken by the garbage collector
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")] // ReSharper disable NotAccessedField.Local
        private Timer renewFederationMetaData;

        // ReSharper restore NotAccessedField.Local
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersFederation"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection String.
        /// </param>
        /// <param name="retryPolicy"> The retry policy</param>
        public UsersFederation(string connectionString, RetryPolicy retryPolicy)
        {
            this.retryPolicy = retryPolicy ?? RetryPolicyProvider.GetRetryPolicy();
            this.connectionString = connectionString;

            // Create the timer. invoke immediatly
            this.renewFederationMetaData = new Timer(this.GetPartitionLowValuesFromDatabaseCallback, null, TimeSpan.Zero, RenewPeriod);
        }

        /// <summary>
        ///     Controls read access to UsersFederation.PartitionLowValues
        /// </summary>
        /// <returns>usersFederation partitionLowValues</returns>
        internal List<int> GetPartitionLowValues()
        {
            lock (partitionsLocker)
            {
                if (partitionLowValues == null)
                {
                    GetPartitionLowValuesFromDatabase();
                }

                return partitionLowValues.ToList();
            }
        }

        /// <summary>
        ///     Controls write access to UsersFederation.PartitionLowValues
        /// </summary>
        private void GetPartitionLowValuesFromDatabase()
        {
            this.retryPolicy.ExecuteAction(
                () =>
                    {
                        using (UsersEntities context = CreateDbContext())
                        {
                            context.Database.Connection.Open();
                            context.Database.ExecuteSqlCommand("USE FEDERATION ROOT WITH RESET");

                            string sql =
                                string.Format(
                                    @"SELECT convert(varchar(100),range_low) FROM sys.federation_member_distributions WHERE federation_id = (SELECT federation_id FROM sys.federations WHERE name = '{0}')", 
                                    FederationName);

                            IEnumerable<string> values = context.Database.SqlQuery<string>(sql);

                            lock (partitionsLocker)
                            {
                                partitionLowValues = values.Select(int.Parse).ToList();
                            }
                        }
                    });
            lastRefreshedTime = DateTime.Now;
        }

        /// <summary>
        /// Controls write access to UsersFederation.PartitionLowValues
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void GetPartitionLowValuesFromDatabaseCallback(object state)
        {
            try
            {
                GetPartitionLowValuesFromDatabase();
            }
            catch (Exception e)
            {
                if ((DateTime.Now - lastRefreshedTime) > MaxStalePeriod)
                {
                    string msg = string.Format("Couldn't refresh users federation metadata in more then last {0} second", MaxStalePeriod.TotalSeconds);
                    var fexp = new AzureFederationRefreshException(msg, e);
                    Log.Critical(EventCode.UserFederationRefreshError, fexp, msg);
                }
                else
                {
                    Log.Warn("Error getting federation metadata info. Error: {0} ", e);
                }
            }
        }

        /// <summary>
        ///     The get context.
        /// </summary>
        /// <returns>
        ///     The <see cref="UsersEntities" />.
        /// </returns>
        private UsersEntities CreateDbContext()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return new UsersEntities();
            }

            return new UsersEntities(connectionString);
        }
    }
}