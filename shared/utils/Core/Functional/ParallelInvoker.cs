//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lomo.Core.Functional
{
    public static class ParallelInvoker
    {
        public static IList<TResult> RunInParallel<TResult>(IList<Closure<TResult>> closures)
        {
            IList<TResult> results = new List<TResult>();
            if (closures == null || closures.Count == 0)
            {
                return results;
            }
            if (closures.Count > 1)
            {
                var remainingClosures = closures.Skip(1);
                var tasks = remainingClosures.Select(c => Task.Run(() => c())).ToArray();
                var firstClosure = closures.Take(1).First();
                var firstResult = firstClosure();
                results.Add(firstResult);
                Task.WaitAll(tasks);
                tasks.ForEach(t => results.Add(t.Result));
            }
            else
            {
                var result = closures.First()();
                results.Add(result);
            }
            return results;
        }
    }
}