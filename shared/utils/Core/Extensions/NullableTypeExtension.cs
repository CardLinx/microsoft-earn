//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Extensions
{
    public static class NullableTypeExtension
    {
        public static double GetValue (double? inputValue) 
        {
            if (inputValue.HasValue)
            {
                return inputValue.Value;
            }
            return default(double) ;
        }

        public static int GetValue(int? inputValue)
        {
            if (inputValue.HasValue)
            {
                return inputValue.Value;
            }
            return default(int);
        }

    }
}