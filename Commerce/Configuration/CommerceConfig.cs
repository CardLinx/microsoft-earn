//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Configuration
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Web.Configuration;

    /// <summary>
    /// Represents Lomo commerce configuration.
    /// </summary>
    public abstract class CommerceConfig : ConfigurationSection
    {
        /// <summary>
        /// Gets the log verbosity level.
        /// </summary>
        [ConfigurationProperty(LogVerbosityAttribute, DefaultValue = SourceLevels.Warning, IsRequired = false)]
        public SourceLevels LogVerbosity
        {
            get
            {
                return (SourceLevels)this[LogVerbosityAttribute];
            }
        }

        /// <summary>
        /// Gets a value indicating whether mock partner dependencies should be used.
        /// </summary>
        [ConfigurationProperty(UseMockPartnerDependenciesAttribute, DefaultValue = false, IsRequired = false)]
        public bool UseMockPartnerDependencies
        {
            get
            {
                return (bool) this[UseMockPartnerDependenciesAttribute];
            }
        }

        /// <summary>
        /// Gets a value indicating the level of functional mocking for the commerce data store.
        /// </summary>
        [ConfigurationProperty(DataStoreMockLevelAttribute, DefaultValue = CommerceDataStoreMockLevel.None, IsRequired = false)]
        public CommerceDataStoreMockLevel DataStoreMockLevel
        {
            get
            {
                return (CommerceDataStoreMockLevel) this[DataStoreMockLevelAttribute];
            }
        }

        /// <summary>
        /// Gets a value indicating whether the event log will be used in lieu of the logging mechanism typically used in the
        /// current environment.
        /// </summary>
        [ConfigurationProperty(ForceEventLogAttribute, DefaultValue = false, IsRequired = false)]
        public bool ForceEventLog
        {
            get
            {
                return (bool) this[ForceEventLogAttribute];
            }
        }

        /// <summary>
        /// Gets a value indicating whether service health logging will be enabled.
        /// </summary>
        [ConfigurationProperty(EnableServiceHealthAttribute, DefaultValue = false, IsRequired = false)]
        public bool EnableServiceHealth
        {
            get
            {
                return (bool)this[EnableServiceHealthAttribute];
            }
        }

        /// <summary>
        /// Gets the maximum number of times opening a SQL connection will be tried before giving up.
        /// </summary>
        [ConfigurationProperty(MaxSqlConnectionRetriesAttribute, DefaultValue = 3, IsRequired = false)]
        public int MaxSqlConnectionRetries
        {
            get
            {
                return (int)this[MaxSqlConnectionRetriesAttribute];
            }
        }

        /// <summary>
        /// Gets the initial time in milliseconds before retrying opening a SQL connection.
        /// </summary>
        /// <remarks>
        /// The latency between retries doubles from this initial value each time.
        /// </remarks>
        [ConfigurationProperty(InitialSqlConnectionRetryLatencyAttribute, DefaultValue = 1000, IsRequired = false)]
        public int InitialSqlConnectionRetryLatency
        {
            get
            {
                return (int)this[InitialSqlConnectionRetryLatencyAttribute];
            }
        }

        /// <summary>
        /// Gets the name of the queue to use for job scheduling.
        /// </summary>
        [ConfigurationProperty(SchedulerQueueNameAttribute, DefaultValue = "schedulerqueue", IsRequired = false)]
        public string SchedulerQueueName
        {
            get
            {
                return (string)this[SchedulerQueueNameAttribute];
            }
        }

        /// <summary>
        /// Gets the name of the table to use for job scheduling.
        /// </summary>
        [ConfigurationProperty(SchedulerTableNameAttribute, DefaultValue = "schedulertable", IsRequired = false)]
        public string SchedulerTableName
        {
            get
            {
                return (string)this[SchedulerTableNameAttribute];
            }
        }

        /// <summary>
        /// Gets the environment in which the code is currently running.
        /// </summary>
        [ConfigurationProperty(EnvironmentAttribute, DefaultValue = "Test", IsRequired = false)]
        public string Environment
        {
            get
            {
                return (string)this[EnvironmentAttribute];
            }
        }

        /// <summary>
        /// Gets the maximum number of times job execution will be tried before the job is sent to the back of the queue.
        /// </summary>
        [ConfigurationProperty(MaxJobRetriesAttribute, DefaultValue = 3, IsRequired = false)]
        public int MaxJobRetries
        {
            get
            {
                return (int)this[MaxJobRetriesAttribute];
            }
        }

        /// <summary>
        /// Gets the initial time in milliseconds before retrying job execution.
        /// </summary>
        /// <remarks>
        /// The latency between retries doubles from this initial value each time.
        /// </remarks>
        [ConfigurationProperty(InitialJobRetryLatencyAttribute, DefaultValue = 1000, IsRequired = false)]
        public int InitialJobRetryLatency
        {
            get
            {
                return (int)this[InitialJobRetryLatencyAttribute];
            }
        }

        /// <summary>
        /// The delay interval between polling NextEvent within the processing loop.
        /// </summary>
        [ConfigurationProperty(ProcessingLoopPollingIntervalAttribute, DefaultValue = 50, IsRequired = false)]
        public int ProcessingLoopPollingInterval
        {
            get
            {
                if (processingLoopPollingIntervalSet == false)
                {
                    processingLoopPollingInterval = (int)this[ProcessingLoopPollingIntervalAttribute];
                    processingLoopPollingIntervalSet = true;
                }

                return processingLoopPollingInterval;
            }
            internal set
            {
                lock (this)
                {
                    processingLoopPollingInterval = value;
                    processingLoopPollingIntervalSet = true;
                }
            }
        }
        private int processingLoopPollingInterval;
        private bool processingLoopPollingIntervalSet = false;

        /// <summary>
        /// Gets the user services endpoint to use within the user services client.
        /// </summary>
        [ConfigurationProperty(UserServicesClientEndpointAttribute, DefaultValue = "http://www.msn.com", IsRequired = false)]
        public string UserServicesClientEndpoint
        {
            get
            {
                return (string)this[UserServicesClientEndpointAttribute];
            }
        }

        /// <summary>
        /// Gets SmsService EndPoint.
        /// </summary>
        [ConfigurationProperty(SmsServiceClientEndpointAttribute, DefaultValue = "", IsRequired = false)]
        public string SmsServiceClientEndpoint 
        {
            get
            {
                return (string)this[SmsServiceClientEndpointAttribute];
            }
        }

        /// <summary>
        /// Gets a value indicating whether redemption rewards are enabled.
        /// </summary>
        [ConfigurationProperty(EnableRedemptionRewardsAttribute, DefaultValue = true, IsRequired = false)]
        public bool EnableRedemptionRewards
        {
            get
            {
                return (bool)this[EnableRedemptionRewardsAttribute];
            }
        }

        /// <summary>
        /// Gets the ID of the reward to apply for first earn events.
        /// </summary>
        [ConfigurationProperty(FirstEarnRewardIdAttribute, DefaultValue = "B6087E16-B958-499D-A70D-64759B36592F", IsRequired = false)]
        public Guid FirstEarnRewardId
        {
            get
            {
                return (Guid)this[FirstEarnRewardIdAttribute];
            }
        }

        /// <summary>
        /// Gets the amount of the reward to apply for first earn events.
        /// </summary>
        [ConfigurationProperty(FirstEarnRewardAmountAttribute, DefaultValue = 1000, IsRequired = false)]
        public int FirstEarnRewardAmount
        {
            get
            {
                return (int)this[FirstEarnRewardAmountAttribute];
            }
        }

        /// <summary>
        /// Gets the explanation for first earn events.
        /// </summary>
        [ConfigurationProperty(FirstEarnRewardExplanationAttribute, DefaultValue = "Earn Credit First Transaction Bonus", IsRequired = false)]
        public string FirstEarnRewardExplanation
        {
            get
            {
                return (string)this[FirstEarnRewardExplanationAttribute];
            }
        }
        
        /// <summary>
        /// Gets the maximum number of times a partner API will be invoked before giving up.
        /// </summary>
        [ConfigurationProperty(MaxPartnerRetriesAttribute, DefaultValue = 3, IsRequired = false)]
        public int MaxPartnerRetries
        {
            get
            {
                return (int)this[MaxPartnerRetriesAttribute];
            }
        }

        /// <summary>
        /// Gets the initial time in milliseconds before retrying the invocation of a partner API.
        /// </summary>
        /// <remarks>
        /// The latency between retries doubles from this initial value each time.
        /// </remarks>
        [ConfigurationProperty(InitialPartnerRetryLatencyAttribute, DefaultValue = 25, IsRequired = false)]
        public int InitialPartnerRetryLatency
        {
            get
            {
                return (int)this[InitialPartnerRetryLatencyAttribute];
            }
        }
        
        /// <summary>
        /// The commerce configuration element's SchedulerQueueName attribute name.
        /// </summary>
        protected const string SchedulerQueueNameAttribute = "schedulerQueueName";

        /// <summary>
        /// The commerce configuration element's SchedulerTableName attribute name.
        /// </summary>
        protected const string SchedulerTableNameAttribute = "schedulerTableName";

        /// <summary>
        /// The commerce configuration element's log verbosity attribute name.
        /// </summary>
        private const string LogVerbosityAttribute = "logVerbosity";

        /// <summary>
        /// The commerce configuration element's use UseMockPartnerDependencies directive attribute name.
        /// </summary>
        private const string UseMockPartnerDependenciesAttribute = "useMockPartnerDependencies";

        /// <summary>
        /// The commerce configuration element's DataStoreMockLevel directive attribute name.
        /// </summary>
        private const string DataStoreMockLevelAttribute = "dataStoreMockLevel";

        /// <summary>
        /// The commerce configuration element's ForceEventLog directive attribute name.
        /// </summary>
        private const string ForceEventLogAttribute = "forceEventLog";

        /// <summary>
        /// The commerce configuration element's enableServiceHealth attribute name.
        /// </summary>
        private const string EnableServiceHealthAttribute = "enableServiceHealth";

        /// <summary>
        /// The commerce configuration element's MaxSqlConnectionRetries attribute name.
        /// </summary>
        private const string MaxSqlConnectionRetriesAttribute = "maxSqlConnectionRetries";

        /// <summary>
        /// The commerce configuration element's InitialSqlConnectionRetryLatency attribute name.
        /// </summary>
        private const string InitialSqlConnectionRetryLatencyAttribute = "initialSqlConnectionRetryLatency";

        /// <summary>
        /// The commerce configuration element's ProcessingLoopPollingInterval attribute name.
        /// </summary>
        private const string ProcessingLoopPollingIntervalAttribute = "processingLoopPollingInterval";

        /// <summary>
        /// The commerce configuration element's Environment attribute name.
        /// </summary>
        private const string EnvironmentAttribute = "environment";

        /// <summary>
        /// The commerce configuration element's MaxJobRetries attribute name.
        /// </summary>
        private const string MaxJobRetriesAttribute = "maxJobRetries";

        /// <summary>
        /// The commerce configuration element's InitialJobRetryLatency attribute name.
        /// </summary>
        private const string InitialJobRetryLatencyAttribute = "initialJobRetryLatency";

        /// <summary>
        /// The commerce configuration element's UserServicesClient URI attribute name.
        /// </summary>
        private const string UserServicesClientEndpointAttribute = "userServicesClientEndpoint";

        /// <summary>
        /// The commerce configuration element's SmsServiceClientEndpoint attribute name.
        /// </summary>
        private const string SmsServiceClientEndpointAttribute = "smsServiceClientEndpoint";

        /// <summary>
        /// The commerce configuration element's EnableRedemptionRewards attribute name.
        /// </summary>
        private const string EnableRedemptionRewardsAttribute = "enableRedemptionRewards";

        /// <summary>
        /// The commerce configuration element's FirstEarnRewardId attribute name.
        /// </summary>
        private const string FirstEarnRewardIdAttribute = "firstEarnRewardId";

        /// <summary>
        /// The commerce configuration element's FirstEarnRewardAmount attribute name.
        /// </summary>
        private const string FirstEarnRewardAmountAttribute = "firstEarnRewardAmount";

        /// <summary>
        /// The commerce configuration element's FirstEarnRewardExplanation attribute name.
        /// </summary>
        private const string FirstEarnRewardExplanationAttribute = "firstEarnRewardExplanation";
        
        /// <summary>
        /// The commerce configuration element's MaxPartnerRetries attribute name.
        /// </summary>
        private const string MaxPartnerRetriesAttribute = "maxPartnerRetries";

        /// <summary>
        /// The commerce configuration element's InitialRetryLatency attribute name.
        /// </summary>
        private const string InitialPartnerRetryLatencyAttribute = "initialPartnerRetryLatency";

    }
}