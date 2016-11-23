//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System.Security.Principal;

    public class CustomPrincipal : IPrincipal
    {
        private string[] roles;

        #region Constructors and Destructors

        public CustomPrincipal(CustomIdentity identity)
        {
            this.Identity = identity;
        }

        public CustomPrincipal(CustomIdentity identity, string[] roles)
        {
            this.Identity = identity;
            this.roles = roles;
        }

        #endregion

        #region Public Properties

        public IIdentity Identity { get; private set; }

        #endregion

        #region Public Methods and Operators

        public bool IsInRole(string role)
        {
            return true;
        }

        #endregion
    }
}