//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core
{
	using System.Collections.Generic;

	public static class SafeQueue<T>
	{
		private static readonly Queue<T> _queue = new Queue<T>();

		public static int Count
		{
			get { return _queue.Count; }
		}

		public static void Enqueue(T item)
		{
			lock (_queue)
				_queue.Enqueue(item);
		}

		public static T TryDequeue()
		{
			lock (_queue)
				return _queue.Count == 0 ? default(T) : _queue.Dequeue();
		}

		public static IList<T> Dequeue(int count)
		{
			var result = new List<T>(count);
			lock (_queue)
				while (result.Count < count && _queue.Count > 0)
					result.Add(_queue.Dequeue());
			return result;
		}
	}
}