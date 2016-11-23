//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System.Collections.ObjectModel;

    public class StatementCreditFile
    {
        /// <summary>
        /// Gets or sets the Statment Credit File's Header
        /// </summary>
        public StatementCreditHeader Header { get; set; }

        /// <summary>
        /// Gets Statment Credit File's records
        /// </summary>
        public Collection<StatementCreditDetail> StatementCreditRecords
        {
            get
            {
                return statementCreditRecords;
            }
        }
        private Collection<StatementCreditDetail> statementCreditRecords = new Collection<StatementCreditDetail>();

        /// <summary>
        /// Gets or sets the Statment Credit File's Trailer
        /// </summary>
        public StatementCreditTrailer Trailer { get; set; }
    }
}