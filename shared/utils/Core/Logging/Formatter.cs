//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Logging
{
	using System;

	static class Formatter
	{
        internal static string Format(string message)
        {
            return string.Format("\r\n{0} {1}", TimeStamp(), message);
        }

        internal static string Format(Exception e)
        {
            return string.Format("\r\n{0} {1}: {2}\r\n{3}", TimeStamp(), e.GetType().Name, e.Message, e.StackTrace);
        }

        internal static string Format(string message, string level)
        {
            return string.Format("\r\n{0} {1} {2}", level, TimeStamp(), message);
        }

        internal static string Format(Exception e, string level)
        {
            return string.Format("\r\n{0} {1} {2}: {3}\r\n{4}", level, TimeStamp(), e.GetType().Name, e.Message, e.StackTrace);
        }

		private static string TimeStamp()
		{
			return DateTime.Now.ToString("yy-MM-dd HH:mm:ss.ff");
		}
	}
}