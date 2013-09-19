using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree
{
    public class ReflectionUtil
    {
        public static object CreateGenericList(Type elementType, object[] contentsAsObj)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(elementType);

            var list = (IList)Activator.CreateInstance(constructedListType);

            foreach (var obj in contentsAsObj)
            {
                Util.Assert(elementType.IsAssignableFrom(obj.GetType()), 
                    "Wrong type when creating generic list, expected something assignable from '"+ elementType +"', but found '" + obj.GetType() + "'");

                list.Add(obj);
            }

            return list;
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
            return type.GetFields(flags).Concat(GetAllFields(type.BaseType));
        }

        public static List<FieldInfo> GetFieldsWithAttribute<T>(Type type)
        {
            var fields = new List<FieldInfo>();

            foreach (var fieldInfo in GetAllFields(type))
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(T), true);

                if (attrs.Length > 0)
                {
                    fields.Add(fieldInfo);
                }
            }

            return fields;
        }
    }
}

