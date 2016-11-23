//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Extensions
{
	using System.Linq;
	using System.Collections.Generic;

// ReSharper disable InconsistentNaming
	public static class IDictionaryExtensions
// ReSharper restore InconsistentNaming
	{
		public static TValue NullIfNotExist<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key) 
			where TValue : class
		{
			return !self.ContainsKey(key) ? null : self[key];
		}

		public static bool IsEqual<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
		{
			if (first == null)
				first = new Dictionary<TKey, TValue>();
			if (second == null)
				second = new Dictionary<TKey, TValue>();

			var keys = first.Keys;
			if (!keys.SequenceEqual(second.Keys))
				return false;
			return keys.All(key => first[key].Equals(second[key]));
		}

		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IEnumerable<KeyValuePair<TKey, TValue>> kvps, bool overwrite = true)
		{
			kvps.ForEach(kvp =>
				{
					if (!self.ContainsKey(kvp.Key))
						self.Add(kvp);
					else if (overwrite)
						self[kvp.Key] = kvp.Value;
				});
		}

		// this overload is for the case when value is IList<>
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, IList<TValue>> self, IEnumerable<KeyValuePair<TKey, IList<TValue>>> kvps)
		{
			kvps.ForEach(kvp =>
				{
					if (!self.ContainsKey(kvp.Key))
						self.Add(kvp);
					else
						self[kvp.Key] = self[kvp.Key].
							Union(kvp.Value).
							Distinct().
							ToList();
				});
		}
	}
}