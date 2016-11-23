//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Performance
{
	using System;
	using System.Linq;
	using System.Diagnostics;
	using System.Collections.Generic;

	using Logging;
	using Extensions;

	public sealed class Monitor
	{
		private readonly IDictionary<string, Counter> _dictionary = new Dictionary<string, Counter>();
		private readonly Action<IList<NamedCounter>> _flush;
		private readonly TimeSpan _interval;
		private DateTime _nextFlush;

		public Monitor(Action<IList<NamedCounter>> flush, TimeSpan interval)
		{
			_flush = flush;
			_interval = interval;
			_nextFlush = DateTime.Now + _interval;
		}

		public T Measure<T>(Func<T> func, string name)
		{
			if (func == null)
				throw new ArgumentNullException("func");
			if (name == null)
				throw new ArgumentNullException("name");
			var sw = new Stopwatch();
			sw.Start();
			T result = func();
			var elapsed = sw.Elapsed.TotalMilliseconds;
			IList<NamedCounter> list = null;
			lock (_dictionary)
			{
				if (_dictionary.ContainsKey(name))
				{
					var counter = _dictionary[name];
					counter.Value += (elapsed - counter.Value) / ++counter.Count;
				}
				else
					_dictionary.Add(name, new Counter { Count = 1, Value = elapsed });
				// check if we need to flush
				if (DateTime.Now > _nextFlush)
				{
					_nextFlush += _interval;
					list = _dictionary.
						Where(x => x.Value.Count > 0).
						Select(x => new NamedCounter
						{
							Name = x.Key,
							Count = x.Value.Count,
							Value = x.Value.Value
						}).
						ToList();
					// reset the dictionary
					_dictionary.Values.ForEach(x => { x.Count = 0; x.Value = 0; });
				}
			}
			if (list != null && list.Count > 0)
				_flush(list);
			return result;
		}

		public static double Measure(Action action, Stopwatch watch)
		{
			watch.Restart();
			action();
			watch.Stop();
			return watch.Elapsed.TotalMilliseconds;
		}

		public static void LogFlush(IList<NamedCounter> list)
		{
			list.OrderByDescending(x => x.Count).Take(1).ForEach(x => 
				Log.Information("Monitor: {0}: Total: {1}, Average: {2:N1} ms".
					ExpandWith(x.Name, x.Count, x.Value)));
		}

		internal class Counter
		{
			internal int Count;
			internal double Value;
		}

		public struct NamedCounter
		{
			public string Name;
			public int Count;
			public double Value;
		}
	}
}