using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace ModestTree
{
    // Used when running tests
    public class AssertException : Exception
    {
        public AssertException(string message)
            : base(message)
        {
        }
    }

    public enum AssertHandleMethod
    {
        MessageBox,
        Exception, // For running tests
        LogAndContinue,
    }

    public class Util
    {
        static AssertHandleMethod _handleMethod = AssertHandleMethod.MessageBox;

        static bool _isAsserting = false;

        public static void SetAssertHandleMethod(AssertHandleMethod handleMethod)
        {
            _handleMethod = handleMethod;
        }

        // Note: ignore asserts for release builds
        // We add UNITY_EDITOR because unity does not define the debug macro
        // so would not trigger asserts for any .cs files added directly
        // to the project
        [Conditional("DEBUG")]
        [Conditional("UNITY_EDITOR")]
        // For now include asserts in web builds
        [Conditional("UNITY_WEBPLAYER")]
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit!");
            }
        }

        [Conditional("DEBUG")]
        [Conditional("UNITY_EDITOR")]
        // For now include asserts in web builds
        [Conditional("UNITY_WEBPLAYER")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit! " + message);
            }
        }

        [Conditional("DEBUG")]
        [Conditional("UNITY_EDITOR")]
        // For now include asserts in web builds
        [Conditional("UNITY_WEBPLAYER")]
        static void TriggerAssert(string message)
        {
            if (_isAsserting)
            {
                // Avoid infinite loops if our error reporting system asserts
                return;
            }

            switch (_handleMethod)
            {
                case AssertHandleMethod.LogAndContinue:
                    _isAsserting = true;
                    Log.Error(message);
                    _isAsserting = false;
                    break;

                case AssertHandleMethod.Exception:
                    // this is usually used when running unit tests
                    // for some reason using StackAnalyzer does not work in this case, 
                    // so don't return stack trace
                    throw new AssertException(message);

                case AssertHandleMethod.MessageBox:
                    _isAsserting = true;
                    Log.Error(message);
#if UNITY_EDITOR
                    ErrorPopupHandler.Trigger(message);
#else
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
#endif
                    _isAsserting = false;
                    break;
            }
        }
    }
}
