using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zenject
{
    public class Container : IContainer
    {
        Dictionary<Type, List<ProviderInternal>> _providers = new Dictionary<Type, List<ProviderInternal>>();

        SingletonProviderMap _singletonMap;
        List<Type> _lookupsInProgress = new List<Type>();

        public Container()
        {
            _singletonMap = new SingletonProviderMap(this);

            Bind<IContainer>().AsSingle(this);
            Bind<GameObjectFactory>().AsSingle();
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
                ZenUtil.Assert(_providers[typeof(TContract)].Find(item => ReferenceEquals(item, provider)) == null,
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

        private List<object> ResolveInternal(Type contract, ResolveContext context)
        {
            if (_lookupsInProgress.Contains(contract))
            {
                var lookupStack = _lookupsInProgress.Select(t => t.ToString()).Aggregate((i, str) => ":" + str);
                var errorMsg = "Circular dependency detected!  Lookup stack: '" + lookupStack  + "'";
                ZenUtil.Assert(false, errorMsg);
            }
            _lookupsInProgress.Add(contract);

            var objects = (from provider in _providers[contract] where provider.GetCondition()(context) select provider.Get()).ToList();

            _lookupsInProgress.Remove(contract);
            return objects;
        }

        public object ResolveMany(Type contract, ResolveContext context)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            if (_providers.ContainsKey(contract))
            {
                return CreateGenericList(contract, ResolveInternal(contract, context).ToArray());
            }

            // All many-dependencies are optional, return an empty list
            return CreateGenericList(contract, new object[] {});
        }

        public TContract Resolve<TContract>()
        {
            return Resolve<TContract>(new ResolveContext());
        }

        private object CreateGenericList(Type elementType, object[] contentsAsObj)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(elementType);

            var list = (IList)Activator.CreateInstance(constructedListType);

            foreach (var obj in contentsAsObj)
            {
                list.Add(obj);
            }

            return list;
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

                ZenUtil.Assert(objects.Count < 2,
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
            ZenUtil.Assert(_providers.ContainsKey(contract));

            foreach (var provider in _providers[contract])
            {
                provider.OnRemoved();
            }

            _providers.Remove(contract);
        }
    }
}
