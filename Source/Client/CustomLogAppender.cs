/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Logging;

namespace Client
{
    public class CustomLogAppender : ILogAppender
    {
        public void Append(string filePath, int lineNumber, string member, LogLevel level, string message, Exception exception = null)
        {
            if (exception != null)
            {
                System.Console.WriteLine($"{System.DateTime.UtcNow} - ({filePath} - {member}[{lineNumber}]) - {level} - {message} - {exception.Message} - {exception.StackTrace}");
            }
            else 
            {
                System.Console.WriteLine($"{System.DateTime.UtcNow} - ({filePath} - {member}[{lineNumber}]) - {level} - {message}");
            }
        }
    }
}