using System;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree
{
    public class LogStreamUnity : ILogStream
    {
        static readonly LogStreamUnity _instance;

        static LogStreamUnity()
        { 
            _instance = new LogStreamUnity();
        } 

        public static LogStreamUnity Instance
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
            UnityEngine.Debug.Log( message );
        }

        public void Info(LogCategory category, string message, string stackTrace)
        {
            UnityEngine.Debug.Log( message );
        }

        public void Warn(LogCategory category, string message, string stackTrace)
        {
            UnityEngine.Debug.LogWarning( message );
        }

        public void Error(LogCategory category, string message, string stackTrace)
        {
            UnityEngine.Debug.LogError( message + "\n" + stackTrace );
        }
    }
}
