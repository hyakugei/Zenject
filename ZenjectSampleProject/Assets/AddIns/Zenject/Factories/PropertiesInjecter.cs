﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    // Iterate over fields/properties on a given object and inject any with the [Inject] attribute
    class PropertiesInjecter
    {
        DiContainer _container;
        List<object> _additional;

        public PropertiesInjecter(DiContainer container)
        {
            _container = container;
            _additional = new List<object>();
        }

        public PropertiesInjecter(DiContainer container, List<object> additional)
        {
            _container = container;
            _additional = additional;
        }

        public void Inject(object injectable)
        {
            Assert.That(injectable != null);

            var fields = ZenUtil.GetFieldDependencies(injectable.GetType());

            var parentDependencies = new List<Type>(_container.LookupsInProgress);

            foreach (var fieldInfo in fields)
            {
                var injectInfo = ZenUtil.GetInjectInfo(fieldInfo);
                Assert.That(injectInfo != null);

                bool foundAdditional = false;
                foreach (object obj in _additional)
                {
                    if (fieldInfo.FieldType.IsAssignableFrom(obj.GetType()))
                    {
                        fieldInfo.SetValue(injectable, obj);
                        _additional.Remove(obj);
                        foundAdditional = true;
                        break;
                    }
                }
                if (foundAdditional)
                {
                    continue;
                }

                var context = new ResolveContext()
                {
                    Target = injectable.GetType(),
                    FieldName = fieldInfo.Name,
                    Name = injectInfo.Name,
                    Parents = parentDependencies,
                    TargetInstance = injectable,
                };

                var valueObj = ResolveField(fieldInfo, context, injectInfo);

                Assert.That(valueObj != null || injectInfo.Optional, () =>
                        "Unable to find field with type '" + fieldInfo.FieldType +
                        "' when injecting dependencies into '" + injectable + "'. \nObject graph:\n" + _container.GetCurrentObjectGraph());

                fieldInfo.SetValue(injectable, valueObj);
            }
        }

        object ResolveField(FieldInfo fieldInfo, ResolveContext context, ZenUtil.InjectInfo injectInfo)
        {
            var desiredType = fieldInfo.FieldType;
            var valueObj = _container.TryResolve(desiredType, context);

            if (valueObj == null)
            {
                // If it's a list it might map to a collection
                if (ReflectionUtil.IsGenericList(desiredType))
                {
                    var subTypes = desiredType.GetGenericArguments();

                    if (subTypes.Length == 1)
                    {
                        var subType = subTypes[0];

                        // Dependencies that are lists are only optional if declared as such using the inject attribute
                        bool optional = (injectInfo == null ? false : injectInfo.Optional);

                        valueObj = _container.ResolveMany(subType, context, optional);
                    }
                }
            }

            return valueObj;
        }
    }
}
