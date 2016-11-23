//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Serialization
{
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;

	public static class BinarySerializer
	{
		public static byte[] Serialize<T>(T data)
		{
			using (var stream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(stream, data);
				return stream.ToArray();
			}
		}

		public static T Deserialize<T>(byte[] bytes) where T : class
		{
			using (var stream = new MemoryStream(bytes))
			{
				return new BinaryFormatter().Deserialize(stream) as T;
			}
		}
	}
}