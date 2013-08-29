using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;

namespace ModestTree.Zenject
{
    public abstract class FactoryBase<T> : IFactory<T>
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
                    var context = new ResolveContext()
                    {
                        target = concreteType, 
                        name = paramInfo.Name, 
                        parents = new List<Type>(_container.LookupsInProgress)
                    };

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

                    Util.Assert(param != null,
                            "Unable to find parameter with type '" + paramInfo.ParameterType +
                            "' while constructing '" + concreteType + "'");
                    parameters.Add(param);
                }
            }

            Util.Assert(extrasList.Count == 0, "Passed unnecessary parameters when constructing '" + concreteType + "'");

            var newObj = method.Invoke(parameters.ToArray());

            var injecter = new PropertiesInjecter(_container);
            injecter.Inject(newObj);

            return newObj;
        }
    }

    public class Factory<TContract, TConcrete> : FactoryBase<TContract> where TConcrete : TContract
    {
        public Factory(IContainer container)
            : base(container)
        {
        }

        public override TContract Create(params object[] constructorArgs)
        {
            return (TContract)Create(typeof(TConcrete), constructorArgs);
        }
    }

    public class Factory<TContract> : FactoryBase<TContract>
    {
        public Factory(IContainer container)
            : base(container)
        {
        }

        public override TContract Create(params object[] constructorArgs)
        {
            return (TContract)Create(typeof(TContract), constructorArgs);
        }
    }
}
