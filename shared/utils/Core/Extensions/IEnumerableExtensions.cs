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
	public static class IEnumerableExtensions
// ReSharper restore InconsistentNaming
	{
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var t in enumerable)
				action(t);
		}

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			return enumerable == null || !enumerable.Any();
		}
	}
}