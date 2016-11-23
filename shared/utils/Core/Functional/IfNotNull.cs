//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Functional
{
	using System;

	public static class IfNotNull
	{
		public static TOut GetValue<TIn, TOut>(TIn data, Func<TIn, TOut> func) 
			where TIn : class
			where TOut : class
		{
			return data == null ? null : func(data);
		}
	}

	public static class Test
	{
		public static void TestIt()
		{
			var ex = new Exception("Message");
			var value = IfNotNull.GetValue(ex, x => x.Message);
			if (value != "Message")
				throw new Exception("Test failed");
			ex = null;
			value = IfNotNull.GetValue(ex, x => x.Message);
			if (value != null)
				throw new Exception("Test failed");
		}
	}
}