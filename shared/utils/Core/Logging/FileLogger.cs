//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Logging
{
    using System;
    using System.IO;

    public sealed class FileLogger : ILog
    {
        private readonly string _path;

        public FileLogger(string path)
        {
            _path = path;
        }

        public void Information(string message)
        {
            if (_path != null)
                File.AppendAllText(_path, Formatter.Format(message, "INF"));
        }

        public void Warning(string message)
        {
            if (_path != null)
                File.AppendAllText(_path, Formatter.Format(message, "WRN"));
        }

        public void Error(string message)
        {
            if (_path != null)
                File.AppendAllText(_path, Formatter.Format(message, "ERR"));
        }

        public void Error(Exception e)
        {
            if (_path != null)
                File.AppendAllText(_path, Formatter.Format(e, "ERR"));
        }
    }
}