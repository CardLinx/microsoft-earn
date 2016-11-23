//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Lomo.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetAllExceptionMessagesJoined(this Exception exception)
        {
            var allExceptionMessages = exception.GetAllExceptionMessages();
            var exceptionMessagesJoined = string.Join("|", allExceptionMessages);
            return exceptionMessagesJoined;
        }

        public static string GetAllExceptionMessagesAndDictionaryDataJoined(this Exception exception)
        {
            var allExceptionMessages = exception.GetAllExceptionMessagesJoined();
            var exceptionDictionaryData = exception.GetExceptionDictionaryData();
            if (exceptionDictionaryData != null)
            {
                allExceptionMessages = exceptionDictionaryData + Environment.NewLine + exceptionDictionaryData;
            }
            return allExceptionMessages;
        }
        
        public static IEnumerable<string> GetAllExceptionMessages(this Exception exception)
        {
            var ex = exception;
            while (ex != null)
            {
                yield return ex.Message;
                ex = ex.InnerException;
            }
        }

        public static string GetExceptionDictionaryData(this Exception exception)
        {
            string exceptionData = null;
            if (exception.Data.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (DictionaryEntry kvp in exception.Data)
                {
                    sb.Append(kvp.Key);
                    sb.Append("=");
                    sb.Append(kvp.Value);
                    sb.Append("|");
                }
                exceptionData = sb.ToString();
            }
            return exceptionData;
        }
    }
}