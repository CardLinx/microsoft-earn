//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Logging
{
	using System;
	using System.Collections.Generic;

	public sealed class MemoryLogger : ILog
	{
		private readonly List<string> _list = new List<string>();

        public void Information(string message)
        {
            _list.Add((Formatter.Format(message, "INF")));
        }

        public void Warning(string message)
        {
            _list.Add(Formatter.Format(message, "WRN"));
        }

        public void Error(string message)
        {
            _list.Add(Formatter.Format(message, "ERR"));
        }

        public void Error(Exception e)
        {
            _list.Add(Formatter.Format(e, "ERR"));
        }
	}
}