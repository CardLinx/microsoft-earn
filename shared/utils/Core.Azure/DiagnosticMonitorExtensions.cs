//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Azure
{
	using System;
	using System.Linq;
	using System.Diagnostics;
	using Microsoft.WindowsAzure.Diagnostics;

	using Extensions;

	public static class DiagnosticMonitorExtensions
	{
		public static void AddPerformanceCounters(
			this DiagnosticMonitorConfiguration config, 
			CounterCreationDataCollection counters,
			string category)
		{
			counters.				
				Cast<CounterCreationData>().
				Select(x => new PerformanceCounterConfiguration
				{
					CounterSpecifier = @"\{0}\{1}".ExpandWith(category, x.CounterName),
					SampleRate = TimeSpan.FromSeconds(10)
				}).
				ToList().
				ForEach(x => config.PerformanceCounters.DataSources.Add(x));
		}
	}
}