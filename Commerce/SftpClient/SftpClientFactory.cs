//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.SftpClient
{
    using System;
    using System.Collections.Generic;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;

    /// <summary>
    /// Factory to create instances of Sftp Clients
    /// </summary>
    public static class SftpClientFactory
    {
        public static ISftpClient SftpClient(string username, string password, string uri, CommerceConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config", "Parameter config cannot be null.");
            }
            if (sftpClient == null)
            {
                sftpClient = new DefaultSftpClient(username, password, uri);
                if (config.UseMockPartnerDependencies)
                {
                    sftpClient = LateBinding.BuildObjectFromLateBoundAssembly<ISftpClient>("MockSftpClient",
                                                                                         LateBoundMocksAssemblyTypes);
                }
            }

            return sftpClient;
        }
        private static ISftpClient sftpClient;

        /// <summary>
        /// Gets the Types that exist within the specified mocks assembly.
        /// </summary>
        private static IEnumerable<Type> LateBoundMocksAssemblyTypes
        {
            get
            {
                if (lateBoundMocksAssemblyTypes == null)
                {
                    lateBoundMocksAssemblyTypes = LateBinding.GetLateBoundAssemblyTypes(MocksAssemblyName);
                }

                return lateBoundMocksAssemblyTypes;
            }
        }
        private static IEnumerable<Type> lateBoundMocksAssemblyTypes;

        /// <summary>
        /// The fully qualified name of the mocks assembly.
        /// </summary>
        private const string MocksAssemblyName = "Lomo.Commerce.Test.Mocks.dll";
    }
}