//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.MicrosoftPractices
{
	using System;
	using Microsoft.Practices.TransientFaultHandling;

	public class DefaultWebRequestRetryStrategy : ExponentialBackoff
	{
		public DefaultWebRequestRetryStrategy() : base(
			5,
			TimeSpan.FromMilliseconds(500),
			TimeSpan.FromSeconds(60),
			TimeSpan.FromMilliseconds(500))
		{}
	}
}