//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Reflection
{
	using System;

	public class StackFrame
	{
		public static string GetCallingMethodName(int skipFrames)
		{
			var frame = new System.Diagnostics.StackFrame(skipFrames, false);
			return frame.GetMethod().Name;
		}

		public static Type GetCallingMethodDeclaringType(int skipFrames)
		{
			var frame = new System.Diagnostics.StackFrame(skipFrames, false);
			return frame.GetMethod().DeclaringType;
		}
	}
}