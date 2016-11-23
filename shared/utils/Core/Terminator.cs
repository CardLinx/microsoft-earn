//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core
{
	using System.Threading;

	public class Terminator
	{
		public bool IsTerminating;
		public ManualResetEvent Signal = new ManualResetEvent(false);
	}
}