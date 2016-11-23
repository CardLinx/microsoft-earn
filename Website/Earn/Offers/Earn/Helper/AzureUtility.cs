//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Earn.Offers.Earn.Helper
{
    public static class AzureUtility
    {
        private static bool m_IsRunningAzure = GetIsRunningInAzure();

        private static bool GetIsRunningInAzure()
        {
            Guid guidId;
            if (RoleEnvironment.IsAvailable && Guid.TryParse(RoleEnvironment.DeploymentId, out guidId))
                return true;
            return false;
        }

        public static bool IsRunningInAzure()
        {
            return m_IsRunningAzure;
        }

        private static bool m_IsRunningAzureOrDevFabric = GetIsRunningInAzureOrDevFabric();

        private static bool GetIsRunningInAzureOrDevFabric()
        {
            return RoleEnvironment.IsAvailable;
        }

        public static bool IsRunningInAzureOrDevFabric()
        {
            return m_IsRunningAzureOrDevFabric;
        }
    }
}