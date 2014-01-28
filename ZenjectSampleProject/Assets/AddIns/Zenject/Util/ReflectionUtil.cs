using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    public static class ReflectionUtil
    {
        public static bool IsGenericList(Type type)
        {
            return type.FullName.StartsWith("System.Collections.Generic.List");
        }

        public static bool IsGenericList(Type type, out Type contentsType)
        {
            if (IsGenericList(type))
            {
                var genericArgs = type.GetGenericArguments();
                Assert.IsEqual(genericArgs.Length, 1);
                contentsType = genericArgs[0];
                return true;
            }

            contentsType = null;
            return false;
        }

        public static object CreateGenericList(Type elementType, object[] contentsAsObj)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(elementType);

            var list = (IList)Activator.CreateInstance(constructedListType);

            foreach (var obj in contentsAsObj)
            {
                Assert.That(elementType.IsAssignableFrom(obj.GetType()),
                "Wrong type when creating generic list, expected something assignable from '"+ elementType +"', but found '" + obj.GetType() + "'");

                list.Add(obj);
            }

            return list;
        }

        public static AttributeType GetAttribute<AttributeType>(Type type) where AttributeType : class
        {
            var attributes = type.GetCustomAttributes(typeof(AttributeType), true);

            if (attributes.Length == 0)
            {
                return null;
            }

            Assert.IsEqual(attributes.Length, 1, "Expected non-muliple attribute");
            return attributes[0] as AttributeType;
        }

        public static AttributeType GetAttribute<AttributeType>(ParameterInfo paramInfo) where AttributeType : class
        {
            var attributes = paramInfo.GetCustomAttributes(typeof(AttributeType), true);

            if (attributes.Length == 0)
            {
                return null;
            }

            Assert.IsEqual(attributes.Length, 1, "Expected non-muliple attribute");
            return attributes[0] as AttributeType;
        }

        public static bool HasAttribute<AttributeType>(FieldInfo fieldInfo) where AttributeType : class
        {
            var attributes = fieldInfo.GetCustomAttributes(typeof(AttributeType), true);

            return attributes.Length > 0;
        }

        public static AttributeType GetAttribute<AttributeType>(FieldInfo fieldInfo) where AttributeType : class
        {
            var attributes = fieldInfo.GetCustomAttributes(typeof(AttributeType), true);

            if (attributes.Length == 0)
            {
                return null;
            }

            Assert.IsEqual(attributes.Length, 1, "Expected non-muliple attribute");
            return attributes[0] as AttributeType;
        }

        public static object DowncastList<TFrom, TTo>(IEnumerable<TFrom> fromList) where TTo : class, TFrom
        {
            var toList = new List<TTo>();

            foreach (var obj in fromList)
            {
                toList.Add(obj as TTo);
            }

            return toList;
        }

        public static List<string> GetFieldNamesWithAttribute<T>(Type type)
        {
            var names = new List<string>();

            foreach (var field in GetFieldsWithAttribute<T>(type))
            {
                names.Add(field.Name);
            }

            return names;
        }

        public static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            // Recursion is necessary since otherwise we won't get private members in base classes
            var baseClassFields = GetAllFields(type.BaseType);

            return type.GetFields(flags).Concat(baseClassFields);
        }

        public static IEnumerable<FieldInfo> GetFieldsWithAttribute<T>(Type type)
        {
            foreach (var fieldInfo in GetAllFields(type))
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(T), true);

                if (attrs.Length > 0)
                {
                    yield return fieldInfo;
                }
            }
        }

        public static string GetPrettyNameForType<T>()
        {
            return GetPrettyNameForType(typeof(T));
        }

        public static string GetPrettyNameForType(Type type)
        {
            if (type.GetGenericArguments().Length == 0)
            {
                return type.Name;
            }
            var genericArguments = type.GetGenericArguments();
            var typeDefeninition = type.Name;
            var unmangledName = typeDefeninition.Substring(0, typeDefeninition.IndexOf("`"));
            return unmangledName + "<" + String.Join(",", genericArguments.Select<Type,string>(GetPrettyNameForType).ToArray()) + ">";
        }

        public static object GetDefaultForType(Type type)
        {
            if (type == typeof(string))
            {
                return "";
            }

            if(type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}


