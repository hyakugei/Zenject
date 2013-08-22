using System;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree
{
    [Flags]
    public enum LogCategory
    {
        Console = 1,
        General = 2,
        GUI = 4,
        Camera = 8, 
    }

    public interface ILogStream
    {
        void Trace(LogCategory category, string message, string stackTrace);
        void Error(LogCategory category, string message, string stackTrace);
        void Warn(LogCategory category, string message, string stackTrace);
        void Info(LogCategory category, string message, string stackTrace);
        void Debug(LogCategory category, string message, string stackTrace);
    }
}
