//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Interfaces
{
	using System;
	using System.Threading.Tasks;

	public interface IRunnable
	{
		Task Start(Func<bool> terminate);
	}
}