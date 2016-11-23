//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Threading
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Collections.Generic;

	public abstract class BaseSignalledRunnable
	{
		public Task Start(ManualResetEvent terminator)
		{
			var task = new Task(() => ThreadProc(terminator), TaskCreationOptions.LongRunning);
			task.Start();
			return task;
		}

		public IList<Task> Start(ManualResetEvent terminator, int threadCount)
		{
			return Enumerable.Range(0, threadCount).Select(_ => Start(terminator)).ToList();
		}

		protected abstract void ThreadProc(ManualResetEvent terminator);
	}
}