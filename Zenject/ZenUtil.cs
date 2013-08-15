using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace Zenject
{
    public enum AssertHandleMethod
    {
        LogAndContinue,
        Exception, // For running tests
    }

    public class ZenUtil
    {
        private static AssertHandleMethod _handleMethod = AssertHandleMethod.LogAndContinue;

        public static void SetAssertHandleMethod(AssertHandleMethod handleMethod)
        {
            _handleMethod = handleMethod;
        }

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                FailAssert("");
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                FailAssert(message);
            }
        }

        [Conditional("UNITY_EDITOR")]
        static void FailAssert(string msg)
        {
            msg = "Hit Assert in Zenject! " + msg;
            UnityEngine.Debug.LogError(msg);

            if (_handleMethod == AssertHandleMethod.Exception)
            {
                throw new InvalidOperationException(msg);
            }
        }

        public static InjectAttribute GetInjectAttribute(MemberInfo member)
        {
            var attrs = member.GetCustomAttributes(typeof(InjectAttribute), true);
            InjectAttribute injectAttr = null;

            foreach (var attr in attrs)
            {
                if (attr.GetType() == typeof(InjectAttribute))
                {
                    injectAttr = attr as InjectAttribute;
                    break;
                }
            }

            return injectAttr;
        }

        public static MemberInfo[] GetMemberDependencies(Type type)
        {
            var allMembers = type.FindMembers(MemberTypes.Property | MemberTypes.Field,
                    BindingFlags.FlattenHierarchy | BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.SetProperty |
                    BindingFlags.Instance,
                    null, null);

            var injectableMembers = new List<MemberInfo>();

            foreach (var member in allMembers)
            {
                if (GetInjectAttribute(member) != null)
                {
                    injectableMembers.Add(member);
                }
            }

            return injectableMembers.ToArray();
        }

        public static ParameterInfo[] GetConstructorDependencies(Type concreteType)
        {
            ConstructorInfo method;
            return GetConstructorDependencies(concreteType, out method);
        }

        public static ParameterInfo[] GetConstructorDependencies(Type concreteType, out ConstructorInfo method)
        {
            var constructors = concreteType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            ZenUtil.Assert(constructors.Length > 0,
                    "Could not find constructor for type '" + concreteType + "' when creating dependencies");
            ZenUtil.Assert(constructors.Length == 1,
                    "More than one constructor found for type '" + concreteType + "' when creating dependencies");

            method = constructors[0];
            return method.GetParameters();
        }
    }
}
