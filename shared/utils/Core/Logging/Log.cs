//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Logging
{
    using System;

    public static class Log
    {
        public static ILog Logger 
        { 
            set { _logger = value; }
        }
        private static ILog _logger;

        public static void Information(string message)
        {
            _logger.Information(message);
        }

        public static void Warning(string message)
        {
            _logger.Warning(message);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Error(Exception e)
        {
            _logger.Error(e);
        }
    }
}