//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.MicrosoftPractices
{
	using Microsoft.Practices.TransientFaultHandling;

	using Logging;
	using Extensions;

	using StackFrame = Reflection.StackFrame;

	public sealed class DefaultWebRequestRetryPolicy : RetryPolicy
	{
		public DefaultWebRequestRetryPolicy() : base(
			new DefaultErrorDetectionStrategy(),
			new DefaultWebRequestRetryStrategy())
		{
			const string MESSAGE = "Retrying {0} attempt {1} for error {2}";
			Retrying += (x, y) => Log.Warning(MESSAGE.ExpandWith(
				StackFrame.GetCallingMethodName(4),
				y.CurrentRetryCount,
				y.LastException.Message));
		}
	}
}