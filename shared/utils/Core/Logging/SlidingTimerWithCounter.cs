//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Logging
{
	using System;

	public sealed class SlidingTimerWithCounter
	{
		private int _count;
		private DateTime _lastActionTime = DateTime.Now;
		private readonly int _intervalSeconds;
		private readonly Action<int, double> _action;

		public SlidingTimerWithCounter(int intervalSeconds, Action<int, double> action)
		{
			_intervalSeconds = intervalSeconds;
			_action = action;
		}

		public void Tick(bool incrementCounter = true, int increment = 1)
		{
			lock (_action)
			{
				if (incrementCounter)
					_count += increment;
				var elapsedSeconds = (DateTime.Now - _lastActionTime).TotalSeconds;
				if (elapsedSeconds > _intervalSeconds)
				{
					_lastActionTime = DateTime.Now;
					_action(_count, elapsedSeconds);
					_count = 0;
				}
			}
		}
	}
}