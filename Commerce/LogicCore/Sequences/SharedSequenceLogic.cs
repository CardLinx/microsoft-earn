//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains helper methods to perform shared business logic for Sequence objects.
    /// </summary>
    public class SharedSequenceLogic
    {
        /// <summary>
        /// Initializes a new instance of the SharedSequenceLogic class.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        /// <param name="sequenceOperations">
        /// The object to use to perform operations on sequences.
        /// </param>
        public SharedSequenceLogic(CommerceContext context,
                               ISequenceOperations sequenceOperations)
        {
            Context = context;
            SequenceOperations = sequenceOperations;
        }

        /// <summary>
        /// Retirieves the next sequence value
        /// </summary>
        /// <returns>
        /// The next value
        /// </returns>
        public int RetrieveNextValueInSequence()
        {
            int result;
            string sequenceName = (string) Context[Key.SequenceName];
            Context.Log.Verbose("Attempting to retrieve next value in sequence with Name {0} from the data store.", sequenceName);
            result = SequenceOperations.RetrieveNextValue();
            if (result != -1)
            {
                Context.Log.Verbose("Next Value retrieved successfully.");
            }
            else
            {
                Context.Log.Verbose("No Sequence with Name {0} found.", sequenceName);
            }

            return result;
        }

        /// <summary>
        /// Retirieves the previous sequence value
        /// </summary>
        /// <returns>
        /// The previous value
        /// </returns>
        public int DecrementSequence()
        {
            int result;
            string sequenceName = (string)Context[Key.SequenceName];
            Context.Log.Verbose("Attempting to decrement the value in sequence with Name {0} from the data store.", sequenceName);
            result = SequenceOperations.DecrementSequenceValue();
            if (result != -1)
            {
                Context.Log.Verbose("Previous Value retrieved successfully.");
            }
            else
            {
                Context.Log.Verbose("No Sequence with Name {0} found.", sequenceName);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context of the current API call.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the object to use to perform operations on sequences.
        /// </summary>
        private ISequenceOperations SequenceOperations { get; set; }
    }
}