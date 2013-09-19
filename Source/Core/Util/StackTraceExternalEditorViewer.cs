using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace ModestTree
{
    // Responsibilities:
    // - Opens an external editor to view the stack trace in
    // - For whatever reason Unity has a limitation on the kinds of external editors it allows, 
    //   so in cases you are using a non-standard editor, you can use this class rather than
    //   just InternalEditorUtility.OpenFileAtLineExternal
    // - This class also outputs the stack trace to a temporary file for cases where the editor
    //   can integrate that into it as well
    public static class StackTraceExternalEditorViewer
    {
        static string _editorCmd;
        static string _editorCmdArguments;

        static StackTraceExternalEditorViewer()
        {
            _editorCmd = Config.Get<string>("ExternalEditor/Command", "");
            _editorCmdArguments = Config.Get<string>("ExternalEditor/Arguments", "");
        }

        public static void OpenExternalEditor(string stackTrace)
        {
            OpenExternalEditor(StackAnalyzer.GetLastFrame(stackTrace), stackTrace);
        }

        public static void OpenExternalEditor(StackAnalyzerFrame frame, string stackTrace)
        {
            OutputStackToTemporaryFile(stackTrace);

            if (_editorCmd.Length > 0 && _editorCmdArguments.Length > 0)
            {
                var args = _editorCmdArguments;
                args = args.Replace("%filePath%", frame.FileName);
                args = args.Replace("%lineNo%", frame.LineNo.ToString());

                var startInfo = new ProcessStartInfo();
                startInfo.FileName = _editorCmd;
                startInfo.Arguments = args;

                Process.Start(startInfo);
            }
            else
            {
#if UNITY_EDITOR
                InternalEditorUtility.OpenFileAtLineExternal(frame.FileName, frame.LineNo);
#endif
            }
        }

        static void OutputStackToTemporaryFile(string stack)
        {
            // Output the stack to a temporary file in case the editor is interested in that as well
            var tempFileDir = System.IO.Path.GetTempPath() + "Unity\\";

            if (!Directory.Exists(tempFileDir))
            {
                Directory.CreateDirectory(tempFileDir);
            }

            var tempFilePath = tempFileDir + "logstack.tmp";
            File.WriteAllText(tempFilePath, stack);
        }
    }
}
