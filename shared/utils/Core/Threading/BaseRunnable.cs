//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Threading
{
	using System;
	using System.Threading.Tasks;

	using Interfaces;

	public abstract class BaseRunnable : IRunnable
	{
		public Task Start(Func<bool> terminate)
		{
			var task = new Task(() => ThreadProc(terminate), TaskCreationOptions.LongRunning);
			task.Start();
			return task;
		}

		protected abstract void ThreadProc(Func<bool> terminate);
	}
}