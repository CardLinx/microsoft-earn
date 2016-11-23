//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;

    /// <summary>
    /// Contains logic necessary to process a FirstData ping request.
    /// </summary>
    public class FirstDataPingExecutor
    {
        /// <summary>
        /// Initializes a new instance of the FirstDataPingExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context for the API being invoked.
        /// </param>
        public FirstDataPingExecutor(CommerceContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Executes processing of the redemption event info.
        /// </summary>
        public void Execute()
        {
            FirstData firstData = new FirstData(Context);
            firstData.BuildPingResponse();
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}