//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Extensions
{
	using System.Linq;
	using System.Text;
	using System.Collections.Generic;

// ReSharper disable InconsistentNaming
	public static class IListExtensions
// ReSharper restore InconsistentNaming
	{
		public static void Add<T>(this IList<T> first, IEnumerable<T> second)
		{
			second.ForEach(first.Add);
		}

		public static string AggregateToString<T>(this IList<T> collection, string separator = ",", string format = "{0}")
		{
			if (collection == null || !collection.Any())
				return null;
			return collection.Aggregate(
				new StringBuilder(), 
				(x, y) => x.Append(separator).Append(format.ExpandWith(y)), 
				x =>
					{
						if (x.Length > 0)
							x = x.Remove(0, separator.Length);
						return x.ToString();
					});
		}

		public static bool IsEqualWithoutOrdering<T>(this IList<T> first, IList<T> second)
		{
			if (first == null)
				first = new List<T>();
			if (second == null)
				second = new List<T>();

			var firstCount = first.Count;
			var secondCount = second.Count;

			if (firstCount != secondCount)
			{
				return false;
			}

		    var commonCount = first.Intersect(second).Count();
		    if (firstCount != commonCount)
		    {
		        return false;
		    }
		    return true;
		}
	}
}