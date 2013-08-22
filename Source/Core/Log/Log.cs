using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ModestTree
{
    // Responsibilities:
    // - Receive log output and send to whatever various log streams are enabled
    public static class Log
    {
        public enum Level
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warn = 3,
            Error = 4 
        }

        static bool _isEnabled = true;
        static List<ILogStream> _streams = new List<ILogStream>();

        static Log()
        {
            LoadStreams();

            Application.RegisterLogCallback(OnUnityLog);

            Info("Initialized log");
        }

        static void LoadStreams()
        {
            Util.Assert(_streams.Count == 0);

            foreach (var streamTypeName in Config.GetAll<string>("Logging/LogStreams/Stream"))
            {
                var type = Type.GetType(streamTypeName);
                Util.Assert(type != null, "Could not resolve type '" + streamTypeName + "'");

                var property = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                Util.Assert(property != null, "Invalid log stream, could not find instance property");

                var stream = property.GetValue(null, null) as ILogStream;
                Util.Assert(stream != null, "Unable to load log stream '" + streamTypeName + "'");

                _streams.Add(stream);
            }

            if (_streams.Count == 0)
            {
                _streams.Add(LogStreamUnity.Instance);

                Log.Warn("No log streams found, defaulting to unity stream");
            }
        }

        public static void AddStream(ILogStream strm)
        {
            Util.Assert(!_streams.Contains(strm));
            _streams.Add(strm);
        }

        public static void RemoveStream(ILogStream strm)
        {
            Util.Assert(_streams.Contains(strm));
            _streams.Remove(strm);
        }

        static void OnUnityLog(string msg, string stackTrace, LogType type)
        {
            if (!_isEnabled)
            {
                return;
            }

            _isEnabled = false; // avoid infinite loops

            stackTrace = StackAnalyzer.PruneStackTrace(stackTrace);

            switch (type)
            {
                case LogType.Log:
                    Info(LogCategory.General, msg, stackTrace);
                    break;

                case LogType.Warning:
                    Warn(LogCategory.General, msg, stackTrace);
                    break;

                case LogType.Assert:
                case LogType.Exception:
                case LogType.Error:
                    Error(LogCategory.General, msg, stackTrace);
#if DEBUG
                    ErrorPopupHandler.Trigger(msg, stackTrace);
#endif
                    break;

                default:
                    Util.Assert(false);
                    break;
            }

            _isEnabled = true;
        }

        public static void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;   
        }

        public static void Trace()
        { 
            Trace(LogCategory.General);
        }

        public static void Trace(LogCategory category)
        {
            if (_isEnabled)
            {
                var frame = StackAnalyzer.GetLastFrame();
                Trace(category, frame.ClassName + ":" + frame.FunctionName);
            }
        }

        public static void Trace(string message)
        {
            if (_isEnabled)
            {
                Trace(LogCategory.General, message);
            }
        }

        public static void Trace(LogCategory category, string message)
        {
            if (_isEnabled)
            {
                _isEnabled = false; // avoid infinite loops

                foreach (var strm in _streams)
                {
                    strm.Trace(category, message, StackAnalyzer.GetStackTrace());
                }

                _isEnabled = true;
            }
        }

        public static void Error(string message)
        {
            if (_isEnabled)
            {
                Error(LogCategory.General, message);
            }
        }

        public static void Error(LogCategory category, string message)
        {
            if (_isEnabled)
            {
                Error(category, message, StackAnalyzer.GetStackTrace());
            }
        }

        static void Error(LogCategory category, string message, string stackTrace)
        {
            _isEnabled = false; // avoid infinite loops

            foreach (var strm in _streams)
            {
                strm.Error(category, message, stackTrace);
            }

            _isEnabled = true;
        }

        public static void Warn(string message)
        {
            if (_isEnabled)
            {
                Warn(LogCategory.General, message);
            }
        }

        public static void Warn(LogCategory category, string message)
        {
            if (_isEnabled)
            {
                Warn(category, message, StackAnalyzer.GetStackTrace());
            }
        }

        static void Warn(LogCategory category, string message, string stackTrace)
        {
            _isEnabled = false; // avoid infinite loops

            foreach (var strm in _streams)
            {
                strm.Warn(category, message, stackTrace);
            }

            _isEnabled = true;
        }

        public static void Info(string message)
        {
            if (_isEnabled)
            {
                Info(LogCategory.General, message);
            }
        }

        public static void Info(LogCategory category, string message)
        {
            if (_isEnabled)
            {
                Info(category, message, StackAnalyzer.GetStackTrace());
            }
        }

        public static void Info(LogCategory category, string message, string stackTrace)
        {
            if (_isEnabled)
            {
                _isEnabled = false; // avoid infinite loops

                foreach (var strm in _streams)
                {
                    strm.Info(category, message, stackTrace);
                }

                _isEnabled = true;
            }
        }

        public static void Debug(string message)
        {
            if (_isEnabled)
            {
                Debug(LogCategory.General, message);
            }
        }

        public static void Debug(LogCategory category, string message)
        {
            if (_isEnabled)
            {
                Debug(category, message, StackAnalyzer.GetStackTrace());
            }
        }

        static void Debug(LogCategory category, string message, string stackTrace)
        {
            _isEnabled = false; // avoid infinite loops

            foreach (var strm in _streams)
            {
                strm.Debug(category, message, stackTrace);
            }

            _isEnabled = true;
        }
    }
}
