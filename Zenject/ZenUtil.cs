using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class ZenUtil
    {
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

            Util.Assert(constructors.Length > 0,
                    "Could not find constructor for type '" + concreteType + "' when creating dependencies");
            Util.Assert(constructors.Length == 1,
                    "More than one constructor found for type '" + concreteType + "' when creating dependencies");

            method = constructors[0];
            return method.GetParameters();
        }
    }
}
