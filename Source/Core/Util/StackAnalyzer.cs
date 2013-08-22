using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ModestTree
{
    public class StackAnalyzerFrame
    {
        public string ClassName;
        public string FunctionName;
        public string FileName;
        public int LineNo;
    }

    // Responsibilities:
    // - Parse a given stack trace and return information such as class/function/line/etc.
    // - Prune parts of the stack trace that probably are not related to the problem (eg. core libraries)
    public static class StackAnalyzer
    {
        static string _ignorePattern = "";

        static StackAnalyzer()
        {
            var patternsToIgnore = Config.GetAll<string>("StackAnalyzer/IgnorePatterns/Pattern");

            foreach (var pattern in patternsToIgnore)
            {
                if (_ignorePattern.Length > 0)
                {
                    _ignorePattern += "|";
                }

                _ignorePattern += pattern;
            }
        }

        public static StackAnalyzerFrame GetLastFrame()
        {
            return GetLastFrame(GetStackTrace());
        }

        // Note: we assume here that the stack trace is generated from UnityEngine.StackTraceUtility.ExtractStackTrace()
        public static StackAnalyzerFrame GetLastFrame(string stackTrace)
        {
            return GetFrame(stackTrace, 0);
        }

        static bool IsRelevantStackTraceLine(string line)
        {
            return (!Regex.Match(line, _ignorePattern, RegexOptions.IgnoreCase).Success);
        }

        public static StackAnalyzerFrame GetFrame(string stackTrace, int index)
        {
            if (stackTrace.Length == 0)
            {
                return null;
            }

            var lines = Regex.Split(stackTrace, "\r\n|\r|\n");

            if (index >= lines.Length)
            {
                return null;
            }

            return ParseLine(lines[index]);
        }

        public static string GetStackTrace()
        {
            return PruneStackTrace(UnityEngine.StackTraceUtility.ExtractStackTrace());
        }

        public static string GetPrettyStackTrace(string stackTrace)
        {
            var relevantLines = new List<string>();
            var allLines = Regex.Split(stackTrace, "\r\n|\r|\n").ToList();

            var prettyStackTrace = "";

            foreach (var line in allLines)
            {
                var frame = ParseLine(line);

                if (frame != null)
                {
                    var className = frame.ClassName;

                    var namespaceSeperator = className.LastIndexOf(".");

                    if (namespaceSeperator != -1)
                    {
                        // remove namespace if there
                        className = className.Substring(namespaceSeperator+1);
                    }

                    prettyStackTrace += className + ":" + frame.FunctionName + "():" + frame.LineNo + "\n";
                }
            }

            return prettyStackTrace;
        }

        public static string PruneStackTrace(string stackTrace)
        {
            var relevantLines = new List<string>();
            var allLines = Regex.Split(stackTrace, "\r\n|\r|\n").ToList();

            foreach (var line in allLines)
            {
                if (IsRelevantStackTraceLine(line))
                {
                    relevantLines.Add(line);
                }
            }

            stackTrace = String.Join("\n", relevantLines.ToArray());
            return stackTrace;
        }

        private static StackAnalyzerFrame ParseLine(string stackLine)
        {
            if (stackLine.Length == 0)
            {
                return null;
            }

            Match match = Regex.Match(stackLine, @"([^:(.]*):?\.?([^.:(]*)\(.*\(at (.*):(\d*)\)$", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return null;
            }

            int lineNo;
            // TryParse because we don't care if it fails (which sometimes happens)
            Int32.TryParse(match.Groups[4].Value, out lineNo);

            return new StackAnalyzerFrame
            {
                ClassName = match.Groups[1].Value, 
                FunctionName = match.Groups[2].Value, 
                FileName = match.Groups[3].Value, 
                LineNo = lineNo,
            };
        }
    }
}
