//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core
{
	using System;
	using System.Linq;
	using System.Collections.Generic;

	public class ByteArrayComparer : IEqualityComparer<byte[]>
	{
		public bool Equals(byte[] left, byte[] right)
		{
			if (left == null || right == null)
				return left == right;
			return left.SequenceEqual(right);
		}

		public int GetHashCode(byte[] key)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			return key.Sum(x => x);
		}
	}
}