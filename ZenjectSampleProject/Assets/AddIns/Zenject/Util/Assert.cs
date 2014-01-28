using System;
using System.Diagnostics;
using UnityEngine;
using Debug=UnityEngine.Debug;

namespace ModestTree.Zenject
{
    public class Assert
    {
        [Conditional("UNITY_EDITOR")]
        public static void TriggerAssert(string message)
        {
            Debug.LogError(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void That(bool condition, string message)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit! " + message);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void That(bool condition)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit!");
            }
        }

        // Pass a function instead of a string for cases that involve a lot of processing to generate a string
        // This way the processing only occurs when the assert fails
        [Conditional("UNITY_EDITOR")]
        public static void That(bool condition, Func<string> messageGenerator)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit! " + messageGenerator());
            }
        }

        // Use AssertEquals to get better error output (with values)
        [Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right)
        {
            IsEqual(left, right, "");
        }

        // Use AssertEquals to get better error output (with values)
        [Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right, string message)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' but found '" + left.ToString() + "'. " + message);
            }
        }

        // Use AssertEquals to get better error output (with values)
        [Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right, Func<string> messageGenerator)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' but found '" + left.ToString() + "'. " + messageGenerator());
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val, string message)
        {
            if (val == null)
            {
                TriggerAssert("Assert Hit! Found null pointer when value was expected. " + message);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val)
        {
            if (val == null)
            {
                TriggerAssert("Assert Hit! Found null pointer when value was expected");
            }
        }
    }
}
