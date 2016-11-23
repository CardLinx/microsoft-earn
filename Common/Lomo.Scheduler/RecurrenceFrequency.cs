//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    /// <summary>
    /// Represents the unit for how often a recurring job will fire
    /// </summary>
    public enum RecurrenceFrequency
    {
        /// <summary>
        ///  No recurrence, so that we can set this value in JSON
        /// </summary>
        None, 
        
        /// <summary>
        ///  Used only for testing purposes - Should not schedule this frequent a job
        /// </summary>
        Second, 
        
        /// <summary>
        /// Job to execute every x number of minutes
        /// </summary>
        Minute,
        
        /// <summary>
        /// Job to execute every x number of hours 
        ///  </summary>
        Hour,
        
        /// <summary>
        /// Job to execute every x number of days
        /// </summary>
        Day
    }
}