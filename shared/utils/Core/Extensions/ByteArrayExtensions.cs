//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Extensions
{
    using System.Linq;
	using System.Text;

	public static class ByteArrayExtensions
	{
		public static string ToHexString(this byte[] data)
		{
			var builder = new StringBuilder(data.Length * 2);
			data.ToList().ForEach(x => builder.Append(x.ToString("X2")));
			return builder.ToString();
		}
	}
}