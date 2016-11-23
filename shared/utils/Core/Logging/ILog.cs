//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Logging
{
    using System;

    public interface ILog
    {
        void Information(string message);
        void Warning(string message);
        void Error(string message);
        void Error(Exception e);
    }
}