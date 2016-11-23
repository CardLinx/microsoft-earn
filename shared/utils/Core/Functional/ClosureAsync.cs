//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Threading.Tasks;

namespace Lomo.Core.Functional
{
    public delegate Task<TResult> ClosureAsync <TResult>();
    public delegate Task<TResult> ClosureWithParameterAsync<in T1, TResult>(T1 arg1);
    public delegate Task<TResult> ClosureWithParameterAsync<in T1, in T2, TResult>(T1 arg1, T2 arg2);
}