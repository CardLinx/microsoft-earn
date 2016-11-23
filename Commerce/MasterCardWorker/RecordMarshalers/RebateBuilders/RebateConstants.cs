//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    /// <summary>
    /// Contains constants used throughout the rebate parsing classes.
    /// </summary>
    internal static class RebateConstants
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
        /// Microsoft's identifier within MasterCard systems.
        /// </summary>
        public const string MemberIca = "00000014859";
    }
}