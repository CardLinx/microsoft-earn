//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Reflection
{
	using System;
	using System.Linq;

	public static class TypeExtensions
	{
		public static T GetAttribute<T>(this Type type) where T : Attribute
		{
			return type.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
		}
	}
}