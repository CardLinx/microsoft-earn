//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Logging
{
	using System;

	public sealed class ConsoleLogger : ILog
	{
        public void Information(string message)
        {
            Console.Write(Formatter.Format(message, "INF"));
        }

        public void Warning(string message)
        {
            Console.Write(Formatter.Format(message, "WRN"));
        }

        public void Error(string message)
        {
            Console.Write(Formatter.Format(message, "ERR"));
        }

        public void Error(Exception e)
        {
            Console.WriteLine(Formatter.Format(e, "ERR"));
        }
	}
}