using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zenject
{
    // The difference between a factory and a provider:
    // Factories create new instances, providers might return an existing instance
    public abstract class FactoryBase<T>
    {
        protected IContainer _container;

        public abstract T Create(params object[] constructorArgs);

        public FactoryBase(IContainer container)
        {
            _container = container;
        }

        protected object Create(Type concreteType, params object[] constructorArgs)
        {
            ConstructorInfo method;
            var parameterInfos = ZenUtil.GetConstructorDependencies(concreteType, out method);

            var parameters = new List<object>();
            var extrasList = new List<object>(constructorArgs);

            foreach (var paramInfo in parameterInfos)
            {
                var found = false;
                var desiredType = paramInfo.ParameterType;

                foreach (var extra in extrasList)
                {
                    if (desiredType.IsAssignableFrom(extra.GetType()))
                    {
                        found = true;
                        parameters.Add(extra);
                        extrasList.Remove(extra);
                        break;
                    }
                }

                if (!found)
                {
                    var context = new ResolveContext() {target = concreteType, name = paramInfo.Name};
                    var param = _container.Resolve(paramInfo.ParameterType, context);

                    if (param == null)
                    {
                        // If it's a list it might map to a collection
                        if (desiredType.FullName.StartsWith("System.Collections.Generic.List"))
                        {
                            var subTypes = desiredType.GetGenericArguments();

                            if (subTypes.Length == 1)
                            {
                                var subType = subTypes[0];

                                param = _container.ResolveMany(subType, context);
                            }
                        }
                    }

                    ZenUtil.Assert(param != null,
                            "Unable to find parameter with type '" + paramInfo.ParameterType +
                            "' while constructing '" + concreteType + "'");
                    parameters.Add(param);
                }
            }

            ZenUtil.Assert(extrasList.Count == 0, "Passed unnecessary parameters when constructing '" + concreteType + "'");

            var newObj = method.Invoke(parameters.ToArray());

            var injecter = new PropertiesInjecter(_container);
            injecter.Inject(newObj);

            return newObj;
        }
    }
}

