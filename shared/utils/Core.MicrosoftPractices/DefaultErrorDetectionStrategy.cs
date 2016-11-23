//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.MicrosoftPractices
{
	using System;

	using Microsoft.Practices.TransientFaultHandling;

	public class DefaultErrorDetectionStrategy : ITransientErrorDetectionStrategy
	{
		public bool IsTransient(Exception e) { return true; }
	}
}