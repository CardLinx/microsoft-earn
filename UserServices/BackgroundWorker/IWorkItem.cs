//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundWorker
{
    public interface IWorkItem
    {
        /// <summary>
        /// Defines a repeatable unit of work that needs to work as background task.
        /// </summary>
        void ExecuteWorkItem();
    }
}