using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace Zenject
{
    public class ZenUtil
    {
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                UnityEngine.Debug.LogError("Hit Assert in Zenject!");
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                UnityEngine.Debug.LogError("Hit Assert in Zenject! " + message);
            }
        }
    }
}
