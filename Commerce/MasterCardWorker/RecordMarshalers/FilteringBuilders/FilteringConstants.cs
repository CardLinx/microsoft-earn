//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    /// <summary>
    /// Contains constant values used throughout the PTS file building classes.
    /// </summary>
    public static class FilteringConstants
    {
        /// <summary>
        /// The character to use populated omitted fields of Alpha data type.
        /// </summary>
        public const char AlphaOmittedCharacter = ' ';

        /// <summary>
        /// The character to use populated omitted fields of Numeric data type.
        /// </summary>
        public const char NumericOmittedCharacter = '0';

        /// <summary>
        /// The character to use populated omitted fields of Date Time data type.
        /// </summary>
        public const char DateTimeOmittedCharacter = ' ';

        /// <summary>
        /// Microsoft's identifier within MasterCard systems.
        /// </summary>
        public const string MemberIca = "014859";

        /// <summary>
        /// The length of the MRS reserved field.
        /// </summary>
        public const int MrsReservedFieldLength = 50;
    }
}