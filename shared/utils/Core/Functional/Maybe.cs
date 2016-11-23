//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Functional
{
	using System;

	public class Maybe<T> where T : class
	{
		private readonly T _value;

		public T Value
		{
			get { return _value; }
		}

		public Maybe(T value)
		{
			_value = value;
		}

		public Maybe<TOut> IfNotNull<TOut>(Func<T, TOut> apply) where TOut : class
		{
			return new Maybe<TOut>(_value == null ? null : apply(_value));
		}

		public void IfNotNull(Action<T> apply)
		{
			if (_value != null)
				apply(_value);
		}
	}
}