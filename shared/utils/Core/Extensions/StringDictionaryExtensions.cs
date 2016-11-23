//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Extensions
{
	using System;
	using System.Linq;
	using System.Collections.Generic;

	public static class StringDictionaryExtensions
	{
		public static bool ContainsKeyIgnoreCase<T>(this IDictionary<string,T> dictionary, string key)
		{
			var containsKey = false;
			if (dictionary != null)
			{
				containsKey = dictionary.Keys.Any(k => string.Equals(k, key, StringComparison.CurrentCultureIgnoreCase));
			}
			return containsKey;
		}

		public static T GetValueIgnoreKeyCase<T>(this IDictionary<string, T> dictionary, string key)
		{
			var returnValue = default(T);
			if (dictionary != null)
			{
				returnValue =(from kvp in dictionary where string.Equals(kvp.Key, key, StringComparison.CurrentCultureIgnoreCase) select kvp.Value).FirstOrDefault();
			}
			return returnValue;
		}

		public static T GetValueIgnoreKeyCaseThrows<T>(this IDictionary<string, T> dictionary, string key)
		{
			if (!dictionary.ContainsKeyIgnoreCase(key))
			{
				throw new Exception(string.Format("Key {0} not found in Dictionary", key));
			}

			return GetValueIgnoreKeyCase<T>(dictionary, key);
		}

		public static T GetValue<T>(this IDictionary<string, T> dictionary, string key, T defaultValue)
		{
			var returnValue = defaultValue;
			if (dictionary != null)
			{
				T keyValue;
				if (dictionary.TryGetValue(key, out keyValue))
				{
					returnValue = keyValue;
				}
			}
			return returnValue;
		}

		public static T GetTypedValueIgnoreKeyCase<T>(this IDictionary<string, string> dictionary, string key)
		{
			return GetTypedValueIgnoreKeyCase(dictionary, key, default(T));
		}

		public static T GetTypedValueIgnoreKeyCase<T>(this IDictionary<string, string> dictionary, string key, T defaultValue)
		{
			var returnValue = defaultValue;
			var keyValue = dictionary.GetValueIgnoreKeyCase(key);
			if (!string.IsNullOrEmpty(keyValue))
			{
				returnValue = (T)Convert.ChangeType(keyValue, typeof(T));
			}
			return returnValue;
		}

		public static void AddValueKeyNotExist<T>(this IDictionary<string, T> dictionary, string key, T keyValue)
		{
			if (!dictionary.ContainsKeyIgnoreCase(key))
			{
				dictionary.Add(key, keyValue);
			}
		}

		public static void AddUpdateIgnoreKeyCase<T>(this IDictionary<string, T> dictionary, string key, T keyValue)
		{
			var keyName = key;
			if (dictionary.ContainsKeyIgnoreCase(key))
			{
				var existingKey = dictionary.Keys.First(k => string.Equals(k, key, StringComparison.CurrentCultureIgnoreCase));
				dictionary.Remove(existingKey);
				keyName = existingKey;
			}
			dictionary.Add(keyName, keyValue);
		}

		public static void RemoveIgnoreKeyCase<T>(this IDictionary<string, T> dictionary, string key)
		{
			var existingKey = dictionary.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.CurrentCultureIgnoreCase));
			if (existingKey != null)
			{
				dictionary.Remove(existingKey);
			}
		}

	}
}