//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Logging
{
	using System;

	public sealed class SlidingTimer
	{
		private DateTime _lastActionTime = DateTime.Now;
		private readonly int _intervalSeconds;
		private readonly Action<double> _action;

		public SlidingTimer(int intervalSeconds, Action<double> action)
		{
			_intervalSeconds = intervalSeconds;
			_action = action;
		}

		public void Tick()
		{
			var elapsedSeconds = (DateTime.Now - _lastActionTime).TotalSeconds;
			if (elapsedSeconds > _intervalSeconds)
			{
				_lastActionTime = DateTime.Now;
				_action(elapsedSeconds);
			}
		}
	}
}