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
        readonly Dictionary<Type, List<ProviderInternal>> _providers = new Dictionary<Type, List<ProviderInternal>>();

        readonly SingletonProviderMap _singletonMap;

        Stack<Type> _lookupsInProgress = new Stack<Type>();

        // This is the list of concrete types that are in the current object graph
        // Useful for error messages (and complex binding conditions)
        internal Stack<Type> LookupsInProgress
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

        internal string GetCurrentObjectGraph()
        {
            if (_lookupsInProgress.Count == 0)
            {
                return "";
            }
            return _lookupsInProgress.Select(t => ReflectionUtil.GetPrettyNameForType(t)).Reverse().Aggregate((i, str) => i + "\n" + str);
        }

        // This occurs so often that we might as well have a convenience method
        public BindingConditionSetter BindFactory<TContract>()
        {
            return Bind<IFactory<TContract>>().AsSingle<Factory<TContract>>();
        }

        public BindingConditionSetter BindFactory<TContract, TConcrete>() where TConcrete : TContract
        {
            return Bind<IFactory<TContract>>().AsSingle<Factory<TContract, TConcrete>>();
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
                Assert.That(_providers[typeof(TContract)].Find(item => ReferenceEquals(item, provider)) == null,
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
            var instances = new List<object>();

            List<ProviderInternal> providers;
            if (_providers.TryGetValue(contract, out providers))
            {
                foreach (var provider in providers.Where(x => x.GetCondition()(context)))
                {
                    instances.Add(provider.GetInstance());
                }
            }

            return instances;
        }

        public object ResolveMany(Type contract, ResolveContext context)
        {
            return ResolveMany(contract, context, true);
        }

        public object ResolveMany(Type contract, ResolveContext context, bool optional)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            if (_providers.ContainsKey(contract))
            {
                return ReflectionUtil.CreateGenericList(contract, ResolveInternal(contract, context).ToArray());
            }

            Assert.That(optional, () =>
                    "Could not find required dependency with type '" + ReflectionUtil.GetPrettyNameForType(contract) + "' \nObject graph:\n" + GetCurrentObjectGraph());

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

        // Same as Resolve except returns null if it can't find it instead of asserting
        public TContract TryResolve<TContract>()
        {
            return TryResolve<TContract>(new ResolveContext());
        }

        public TContract TryResolve<TContract>(ResolveContext context)
        {
            return (TContract) TryResolve(typeof (TContract), context);
        }

        public object TryResolve(Type contract)
        {
            return TryResolve(contract, new ResolveContext());
        }

        public object TryResolve(Type contract, ResolveContext context)
        {
            return ResolveInternal(contract, context, true);
        }

        // Return single insance of requested type or assert
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
            return ResolveInternal(contract, context, false);
        }

        object ResolveInternal(Type contract, ResolveContext context, bool optional)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            var objects = ResolveInternal(contract, context);

            if (!objects.Any())
            {
                Assert.That(optional, () =>
                    "Unable to resolve type '" + contract + "'. \nObject graph:\n" + GetCurrentObjectGraph());

                return null;
            }

            Assert.That(objects.Count == 1, () =>
                "Found multiple matches when only one was expected for type '" + contract + "'. \nObject graph:\n" + GetCurrentObjectGraph());

            return objects[0];
        }

        public void Release<TContract>()
        {
            Release(typeof (TContract));
        }

        public void Release(Type contract)
        {
            Assert.That(_providers.ContainsKey(contract));

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

            foreach (var fieldInfo in ZenUtil.GetFieldDependencies(contract))
            {
                dependencies.Add(fieldInfo.FieldType);
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
            Assert.IsEqual(types.Count, 1);
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
                    Assert.IsEqual(subTypes.Length, 1);

                    var subType = subTypes[0];
                    dependTypes = ResolveTypeMany(subType);
                }
                else
                {
                    dependTypes = ResolveTypeMany(contractType);
                    Assert.That(dependTypes.Count <= 1);
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
