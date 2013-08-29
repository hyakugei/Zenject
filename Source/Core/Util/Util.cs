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

        // Note: ignores asserts for release builds
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit!");
            }
        }

        // Note: ignores asserts for release builds
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit! " + message);
            }
        }

        [Conditional("UNITY_EDITOR")]
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
#if DEBUG
                    _isAsserting = true;
                    Log.Error(message);
                    ErrorPopupHandler.Trigger(message);
                    _isAsserting = false;
#endif
                    break;
            }
        }
    }
}
