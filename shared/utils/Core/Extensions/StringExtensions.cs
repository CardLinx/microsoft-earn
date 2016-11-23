//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.IO;

namespace Lomo.Core.Extensions
{
	using System;
	using System.Linq;
	using System.Text;
	using System.Globalization;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public static class StringExtensions
	{
		private static readonly Regex WhiteSpace = new Regex("\\s+", RegexOptions.Compiled);
		public static T ToEnum<T>(this string s)
		{
			return (T) Enum.Parse(typeof(T), s);
		}

		public static string ExpandWith(this string format, params object[] p)
		{
			return String.Format(format, p);
		}

		public static string RemoveNonAlphaNumerics(this string s)
		{
			return new string(s.Where(x => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x)).ToArray());
		}

		public static string RemoveNonNumerics(this string s)
		{
			return s == null ? null : new string(s.Where(char.IsDigit).ToArray());
		}

		public static string RemoveMultipleSpaces(this string s)
		{
			var tmp = String.Empty;
			while (s.Length != tmp.Length)
			{
				tmp = s;
				s = s.Replace("  ", " ");
			}
			return s;
		}

		public static string RemoveSubstringsAndNonAlphaNumerics(this string s, IEnumerable<string> subs)
		{
			var tokens = s.Tokenize().Where(t => t.Length > 1 && !subs.Contains(t, StringComparer.InvariantCultureIgnoreCase));
			return tokens.ToList().AggregateToString(" ");
		}

		public static List<string> Tokenize(this string s)
		{
			var result = new List<string>();
			if (s == null)
				return result;
			var builder = new StringBuilder(s.Length);
			foreach (char c in s)
			{
				if (c.IsAlphaNumeric())
				{
					builder.Append(c);
					continue;
				}
				if (builder.Length == 0)
					continue;
				result.Add(builder.ToString());
				builder.Clear();
			}
			if (builder.Length > 0)
				result.Add(builder.ToString());
			return result;
		}

		public static IList<string> TokenizeUsingWhiteSpace(this string s)
		{
			var result = new List<string>();
			if (s == null)
				return result;

			result = WhiteSpace.Split(s.Trim()).ToList();
			return result;
		}

		public static bool IsAlphaNumeric(this char c)
		{
			return char.IsLetterOrDigit(c);
		}

		public static void NormalizeStringProperties(object obj)
		{
			var properties = obj.GetType().GetProperties().Where(x => x.PropertyType == typeof(string));
			foreach (var property in properties)
			{
				var value = property.GetValue(obj) as string;
				property.SetValue(obj, String.IsNullOrWhiteSpace(value) ? null : value.Trim());
			}
		}

		public static byte[] ToByteArray(this string hex)
		{
			if (hex.Length % 2 != 0)
				throw new ArgumentException("Valid hex string cannot have an odd number of digits: {0}".ExpandWith(hex));

			return hex.Aggregate(
				new { Result = new List<byte>(), Builder = new StringBuilder() },
				(acc, c) =>
				{
					acc.Builder.Append(c);
					if (acc.Builder.Length == 2)
					{
						var b = byte.Parse(acc.Builder.ToString(), NumberStyles.HexNumber);
						acc.Result.Add(b);
						acc.Builder.Clear();
					}
					return acc;
				},
				acc => acc.Result.ToArray());
		}

		public static Guid? ToGuid(this string guid)
		{
			Guid? output = null;
			if (!string.IsNullOrEmpty(guid))
			{
				output = Guid.Parse(guid);
			}
			return output;
		}

		public static bool IsValidGuid(this string guid)
		{
			Guid output;
			return Guid.TryParse(guid, out output);
		}

		public static DateTime ToDateTime(this string inputDateTime, DateTime defaultValue)
		{
			var outputValue = defaultValue;
			if (!string.IsNullOrEmpty(inputDateTime))
			{
				outputValue = DateTime.Parse(inputDateTime);
			}
			return outputValue;
		}

		public static string TrimIfNotNull(this string s)
		{
			if (s != null)
			{
				s = s.Trim();
			}
			return s;
		}

		public static IList<string> SplitDistinct(this string s, char delimiter = ',')
		{
			s = s ?? string.Empty;
			s = s.Trim();
			var items = s.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Where(item => !string.IsNullOrEmpty(item)).ToList();
			return items;
		}

		public static IList<string> SplitDistinct(this string s, string delimiter)
		{
			s = s ?? string.Empty;
			s = s.Trim();
			var items = s.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Where(item => !string.IsNullOrEmpty(item)).ToList();
			return items;
		}
		
		
		public static string EscapeDelimiter(this string value, string delimiter = "\"")
		{
			if (value != null)
			{
				if (!string.IsNullOrEmpty(delimiter) && value.IndexOf(delimiter, StringComparison.Ordinal) > -1)
				{
					value = value.Replace(delimiter, delimiter + delimiter);
				}
			}
			return value;
		}

		public static string GetDefaultIfNullOrEmpty(this string value, string defaultValue)
		{
			return string.IsNullOrEmpty(value) ? defaultValue : value;
		}

		public static string UppercaseFirst(this string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			return char.ToUpper(s[0]) + s.Substring(1);
		}

		/// <summary>
		/// A case insenstive replace function.
		/// </summary>
		/// <param name="originalString">The string to examine.(HayStack)</param>
		/// <param name="oldValue">The value to replace.(Needle)</param>
		/// <param name="newValue">The new value to be inserted</param>
		/// <returns>A string</returns>
		public static string CaseInsenstiveReplace(this string originalString, string oldValue, string newValue)
		{
			if (string.IsNullOrEmpty(originalString) || string.IsNullOrEmpty(oldValue))
			{
				return originalString;
			}

			var regEx = new Regex(oldValue,RegexOptions.IgnoreCase | RegexOptions.Multiline);
			return regEx.Replace(originalString, newValue);
		}

		public static string GrammarRectifications(this string originalString)
		{
			if (originalString == null)
			{
				return null;
			}

			// spacing after . and ,
			var modifiedString = Regex.Replace(originalString, @"([,.])([^ .\d])", "$1 $2");

			// remove multiple lines
			modifiedString = Regex.Replace(modifiedString, @"\n[\n]+", "\n\n");
			modifiedString = Regex.Replace(modifiedString, @"(\r\n)+", "\r\n");

			// enter without fullstop
			modifiedString = Regex.Replace(modifiedString, @"([^ .:!,?;\n\r])([ ]*)([\n])", "$1.$3");

			return modifiedString;
		}

		public static string GetFileNameFromUrl(this string url)
		{
			if (string.IsNullOrEmpty(url))
				return string.Empty;

			var index = url.IndexOf("?",StringComparison.OrdinalIgnoreCase);
			if (index != -1)
			{
				url = url.Substring(0, index);
			}

			var fileName = Path.GetFileName(url);
			return fileName;
		}

		public static string GetFileNameWithoutExtensionFromUrl(this string url)
		{
			var fileName = url.GetFileNameFromUrl();
			var fileNameWithoutExtenion = Path.GetFileNameWithoutExtension(fileName);
			return fileNameWithoutExtenion;
		}

		public static string GetFileExtensionFromUrl(this string url)
		{
			var fileName = url.GetFileNameFromUrl();
			var fileExtenion = Path.GetExtension(fileName);
			return fileExtenion;
		}

		public static string RemoveInvalidFileCharacters(this string fileName)
		{
			var invalidFileChars = Path.GetInvalidFileNameChars();
			return invalidFileChars.Aggregate(fileName, (current, invalidFileChar) => current.Replace(invalidFileChar, '_'));
		}
	}
}