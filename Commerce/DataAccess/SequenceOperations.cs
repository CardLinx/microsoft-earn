//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System.Collections.Generic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents operations on Sequence objects within the data store.
    /// </summary>
    public class SequenceOperations : CommerceOperations, ISequenceOperations
    {
        /// <summary>
        /// Given a sequence name, get the next value in the series.
        /// </summary>
        /// <returns>
        /// Next value if sequence exists, other wise -1
        /// </returns>
        public int RetrieveNextValue()
        {
            int nextValue = -1;
            string sequenceName = (string)Context[Key.SequenceName];

            SqlProcedure("GetNextValueFromSequence",
                         new Dictionary<string, object>
                         {
                             { "@SequenceName", sequenceName }
                         },

                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read())
                    {
                        nextValue = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("NextValue"));
                    }
                });

            return nextValue;
        }

        /// <summary>
        /// Given a sequence name, decrement the value to its previous value.
        /// </summary>
        /// <returns>
        /// Previous value
        /// </returns>
        public int DecrementSequenceValue()
        {
            int nextValue = -1;
            string sequenceName = (string)Context[Key.SequenceName];

            SqlProcedure("DecreaseSequenceValue",
                         new Dictionary<string, object>
                         {
                             { "@SequenceName", sequenceName }
                         },

                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read())
                    {
                        nextValue = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("NextValue"));
                    }
                });

            return nextValue;
        }
    }
}