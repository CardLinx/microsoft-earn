//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class GuidUtility
    {
        public static string GenerateShortGuid()
        {
            long accumulator = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                accumulator *= ((int)b + 1);
            }

            return String.Format("{0:x}", accumulator - DateTime.Now.Ticks);
        }
    }
}