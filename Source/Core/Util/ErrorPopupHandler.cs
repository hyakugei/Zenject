using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
#if DEBUG && UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using Debug = UnityEngine.Debug;

namespace ModestTree
{
    // Responsibilities:
    // - Display a popup display to user in the unity editor
    public class ErrorPopupHandler
    {
        static bool _hasStopped;
        static string _suppressPopupPattern = "";

        static ErrorPopupHandler()
        {
            var patternsToIgnore = Config.GetAll<string>("ErrorPopupHandler/IgnorePatterns/Pattern");

            foreach (var pattern in patternsToIgnore)
            {
                if (_suppressPopupPattern.Length > 0)
                {
                    _suppressPopupPattern += "|";
                }

                _suppressPopupPattern += pattern;
            }
        }

        static bool ShouldIgnore(string errorMsg)
        {
            return (Regex.Match(errorMsg, _suppressPopupPattern, RegexOptions.IgnoreCase).Success);
        }

        public static void Trigger(string message)
        {
            Trigger(message, StackAnalyzer.GetStackTrace());
        }

        public static void Trigger(string message, string stackTrace)
        {
            if (_hasStopped)
            {
                // Ignore errors that occur after we set isPlaying to false
                return;
            }

            if (ShouldIgnore(message))
            {
                return;
            }

            var lastFrame = StackAnalyzer.GetFrame(stackTrace, 0);

            if (lastFrame == null)
            // This would happen if we hit an error in one of the core libraries, or somewhere that is set to be ignored (see StackAnalyzer)
            {
                if (EditorUtility.DisplayDialog("Error", message, "Stop", "Ignore"))
                {
                    StopRunning();
                }
            }
            else
            {
                var errorMsg = message + "\n\n" + "Filename: " + lastFrame.FileName + "\nMethod: " + lastFrame.FunctionName + "\nLine: " + lastFrame.LineNo;

                switch (EditorUtility.DisplayDialogComplex("Error", errorMsg, "Go To", "Stop", "Ignore"))
                {
                    case 0:
                    // Go to 
                    {
                        StopRunning();
                        StackTraceExternalEditorViewer.OpenExternalEditor(lastFrame, stackTrace);
                        break;
                    }
                    case 1:
                    // Stop
                    {
                        StopRunning();
                        break;
                    }
                }
            }
        }

        static void StopRunning()
        {
            EditorApplication.isPlaying = false;

            // Don't display logging while being force-killed
            Log.SetEnabled(false);
            _hasStopped = true;
        }
    }
}

#endif
