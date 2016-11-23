//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core
{
	using System;
	using System.Text;

	public class SafeRandom
	{
		private readonly Random _random = new Random((Environment.MachineName + DateTime.Now.Ticks).GetHashCode());

		public int Next(int maxValue)
		{
			lock (_random)
				return _random.Next(maxValue);
		}

		public double NextDouble()
		{
			lock (_random)
				return _random.NextDouble();
		}

		public byte[] NextBytes(int count)
		{
			var result = new byte[count];
			lock (_random)
				_random.NextBytes(result);
			return result;
		}
	
		public string RandomString(int length, char startChar, char endChar)
		{
			StringBuilder builder = new StringBuilder(length);
			var range = endChar - startChar + 1;
			lock (_random)
				for (int i = 0; i < length; i++)
					builder.Append(Convert.ToChar(startChar + _random.Next(range)));
			return builder.ToString();
		}

		public string RandomString(int length)
		{
			return RandomString(length, 'A', 'Z');
		}
	}
}