//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    /// <summary>
    /// Contains constant values used throughout the PTS file building classes.
    /// </summary>
    public static class PtsConstants
    {
        /// <summary>
        /// The length of a record in a PTS file.
        /// </summary>
        internal const int RecordLength = 80;

        /// <summary>
        /// The length of the record sequence number field.
        /// </summary>
        internal const int RecordSequenceNumberLength = 6;

        /// <summary>
        /// The character to use to pad the left side of the record sequence number if necessary.
        /// </summary>
        internal const char RecordSequenceNumberPad = '0';

        /// <summary>
        /// The character to use to pad records.
        /// </summary>
        internal const char FillerPad = ' ';

        /// <summary>
        /// The alternate character to use to pad records.
        /// </summary>
        internal const char AlternateFillerPad = '0';

        /// <summary>
        /// The length of the time field.
        /// </summary>
        internal const int TimeFieldLength = 10;
    }
}