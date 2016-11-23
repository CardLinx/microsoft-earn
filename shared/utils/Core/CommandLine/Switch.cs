//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.CommandLine
{
	using System;
	using System.Collections.Generic;

	using Extensions;

	public static class Switch
	{
		public static string GetSwitchValue(this IList<string> argList, string switchName)
		{
			int position = argList.IndexOf(switchName);
			if (position == -1)
				return null;
			if (argList.Count <= position + 1)
				throw new Exception("Switch {0} should be followed by value".ExpandWith(switchName));
			return argList[position + 1];
		}
	}
}