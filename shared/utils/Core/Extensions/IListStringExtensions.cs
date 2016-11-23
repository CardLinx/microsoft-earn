//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

// ReSharper disable InconsistentNaming
    public static class IListStringExtensions
// ReSharper restore InconsistentNaming
	{
		public static void RemoveIgnoreCase(this IList<string> list, string element)
		{
		    var foundElement = list.FirstOrDefault(e => string.Equals(e, element, StringComparison.OrdinalIgnoreCase));
            if (foundElement != null)
            {
                list.Remove(foundElement);
            }
		}

	}
}