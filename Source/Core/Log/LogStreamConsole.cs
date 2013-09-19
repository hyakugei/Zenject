using System;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree
{
    public class LogStreamConsole : ILogStream
    {
        static readonly LogStreamConsole _instance;

        static LogStreamConsole()
        { 
            _instance = new LogStreamConsole();
        } 

        public static LogStreamConsole Instance
        { 
            get 
            { 
                return _instance; 
            } 
        }

        public void Trace(LogCategory category, string message, string stackTrace)
        {
            // Do nothing
        }

        public void Debug(LogCategory category, string message, string stackTrace)
        {
            Console.Out.WriteLine( message );
        }

        public void Info(LogCategory category, string message, string stackTrace)
        {
            Console.Out.WriteLine( message );
        }

        public void Warn(LogCategory category, string message, string stackTrace)
        {
            Console.Out.WriteLine( message );
        }

        public void Error(LogCategory category, string message, string stackTrace)
        {
            Console.Out.WriteLine( message + "\n" + stackTrace );
        }
    }
}

