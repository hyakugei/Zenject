using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    // Responsibilities:
    // - Expose methods to configure object graph via Bind() methods
    // - Build object graphs via Resolve() method
    public class DiContainer
    {
        Dictionary<Type, List<ProviderInternal>> _providers = new Dictionary<Type, List<ProviderInternal>>();

        SingletonProviderMap _singletonMap;
        List<Type> _lookupsInProgress = new List<Type>();

        public List<Type> LookupsInProgress
        {
            get
            {
                return _lookupsInProgress;
            }
        }

        public DiContainer()
        {
            _singletonMap = new SingletonProviderMap(this);

            Bind<DiContainer>().AsSingle(this);
            Bind<GameObjectInstantiator>().AsSingle();
        }

        public Binder<TContract> Bind<TContract>()
        {
            return new Binder<TContract>(this, _singletonMap);
        }

        public void RegisterProvider<TContract>(ProviderInternal provider)
        {
            if (_providers.ContainsKey(typeof (TContract)))
            {
                // Prevent duplicate singleton bindings:
                Util.Assert(_providers[typeof(TContract)].Find(item => ReferenceEquals(item, provider)) == null,
                            "Found duplicate singleton binding for contract '" + typeof (TContract) + "'");

                _providers[typeof (TContract)].Add(provider);
            }
            else
            {
                _providers.Add(typeof (TContract), new List<ProviderInternal> {provider});
            }
        }

        public List<TContract> ResolveMany<TContract>()
        {
            return ResolveMany<TContract>(new ResolveContext());
        }

        public List<TContract> ResolveMany<TContract>(ResolveContext context)
        {
            return (List<TContract>) ResolveMany(typeof (TContract), context);
        }

        public object ResolveMany(Type contract)
        {
            return ResolveMany(contract, new ResolveContext());
        }

        List<object> ResolveInternal(Type contract, ResolveContext context)
        {
            if (_lookupsInProgress.Contains(contract))
            {
                var lookupStack = _lookupsInProgress.Select(t => t.ToString()).Aggregate((i, str) => ":" + str);
                var errorMsg = "Circular dependency detected!  Lookup stack: '" + lookupStack  + "'";
                Util.Assert(false, errorMsg);
            }
            _lookupsInProgress.Add(contract);

            var objects = (from provider in _providers[contract] where provider.GetCondition()(context) select provider.GetInstance()).ToList();

            _lookupsInProgress.Remove(contract);
            return objects;
        }

        public object ResolveMany(Type contract, ResolveContext context)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            if (_providers.ContainsKey(contract))
            {
                return ReflectionUtil.CreateGenericList(contract, ResolveInternal(contract, context).ToArray());
            }

            // All many-dependencies are optional, return an empty list
            return ReflectionUtil.CreateGenericList(contract, new object[] {});
        }

        public List<Type> ResolveTypeMany(Type contract)
        {
            if (_providers.ContainsKey(contract))
            {
                // TODO: fix this to work with providers that have conditions
                var context = new ResolveContext();

                return (from provider in _providers[contract] where provider.GetCondition()(context) select provider.GetInstanceType()).ToList();
            }

            return new List<Type> {};
        }

        public TContract Resolve<TContract>()
        {
            return Resolve<TContract>(new ResolveContext());
        }

        public TContract Resolve<TContract>(ResolveContext context)
        {
            return (TContract) Resolve(typeof (TContract), context);
        }

        public object Resolve(Type contract)
        {
            return Resolve(contract, new ResolveContext());
        }

        public object Resolve(Type contract, ResolveContext context)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            if (_providers.ContainsKey(contract))
            {
                var objects = ResolveInternal(contract, context);

                Util.Assert(objects.Count < 2,
                            "Found multiple matches when only one was expected for type '" + contract + "'");
                    // We could maybe remove this and just return the first one potentially
                return objects.Count == 0 ? null : objects[0];
            }

            return null;
        }

        public void Release<TContract>()
        {
            Release(typeof (TContract));
        }

        public void Release(Type contract)
        {
            Util.Assert(_providers.ContainsKey(contract));

            foreach (var provider in _providers[contract])
            {
                provider.OnRemoved();
            }

            _providers.Remove(contract);
        }

        public List<Type> GetDependencyContracts<TContract>()
        {
            return GetDependencyContracts(typeof(TContract));
        }

        public List<Type> GetDependencyContracts(Type contract)
        {
            var dependencies = new List<Type>();

            foreach (var param in ZenUtil.GetConstructorDependencies(contract))
            {
                dependencies.Add(param.ParameterType);
            }

            foreach (var member in ZenUtil.GetMemberDependencies(contract))
            {
                if (member is FieldInfo)
                {
                    var info = member as FieldInfo;
                    dependencies.Add(info.FieldType);
                }
                else if (member is PropertyInfo)
                {
                    var info = member as PropertyInfo;
                    dependencies.Add(info.PropertyType);
                }
                else
                {
                    Util.Assert(false);
                }
            }

            return dependencies;
        }

        public Dictionary<Type, List<Type>> CalculateObjectGraph<TRoot>()
        {
            return CalculateObjectGraph(typeof(TRoot));
        }

        public Dictionary<Type, List<Type>> CalculateObjectGraph(Type rootContract)
        {
            var map = new Dictionary<Type, List<Type>>();
            var types = ResolveTypeMany(rootContract);
            Util.Assert(types.Count == 1);
            var rootType = types[0];

            map.Add(rootType, new List<Type>());
            AddToObjectGraph(rootType, map);

            return map;
        }

        void AddToObjectGraph(Type type, Dictionary<Type, List<Type>> map)
        {
            var dependList = map[type];

            foreach (var contractType in GetDependencyContracts(type))
            {
                List<Type> dependTypes;

                if (contractType.FullName.StartsWith("System.Collections.Generic.List"))
                {
                    var subTypes = contractType.GetGenericArguments();
                    Util.Assert(subTypes.Length == 1);

                    var subType = subTypes[0];
                    dependTypes = ResolveTypeMany(subType);
                }
                else
                {
                    dependTypes = ResolveTypeMany(contractType);
                    Util.Assert(dependTypes.Count <= 1);
                }

                foreach (var dependType in dependTypes)
                {
                    dependList.Add(dependType);

                    if (!map.ContainsKey(dependType))
                    {
                        map.Add(dependType, new List<Type>());
                        AddToObjectGraph(dependType, map);
                    }
                }
            }
        }
    }
}
