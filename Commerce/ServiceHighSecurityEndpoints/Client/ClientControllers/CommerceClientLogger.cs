//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Commerce client log.
    /// </summary>
    public static class CommerceClientLogger
    {
        static object lockObject = new object();
        static CommerceLog clientLog;
        static Guid clientLogActivityId = new Guid("8855166b-3e2b-40b7-9a29-df9e335f5d4a");

        /// <summary>
        /// Gets the instance of the client logger.
        /// </summary>
        public static CommerceLog Instance
        {
            get
            {
                if (CommerceLog.LogInstanceSet)
                {
                    if (clientLog == null)
                    {
                        lock (CommerceClientLogger.lockObject)
                        {
                            if (clientLog == null)
                            {
                                clientLog = new CommerceLog(clientLogActivityId, CommerceServiceConfig.Instance.LogVerbosity, General.CommerceLogSource);  
                            }

                        }
                    }
                }

                return clientLog;
            }
        }

        /// <summary>
        /// Gets the status of the logger's availability.
        /// </summary>
        public static bool IsAvailable
        {
            get
            {
                return CommerceLog.LogInstanceSet;
            }
        }
    }
}