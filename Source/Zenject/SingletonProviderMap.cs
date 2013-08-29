using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public class SingletonProviderMap
    {
        private interface ISingletonLazyCreator
        {
            void IncRefCount();
            void DecRefCount();

            object GetInstance();
            Type GetInstanceType();
        }

        private class SingletonLazyCreator<T> : ISingletonLazyCreator
        {
            private int _referenceCount;
            private IFactory<T> _factory;
            private T _instance;
            private SingletonProviderMap _map;

            public SingletonLazyCreator(DiContainer container, SingletonProviderMap map)
            {
                _factory = new Factory<T>(container);
                _map = map;
            }

            public void IncRefCount()
            {
                _referenceCount += 1;
            }

            public void DecRefCount()
            {
                _referenceCount -= 1;

                if (_referenceCount <= 0)
                {
                    _map.Remove<T>();
                }
            }

            public Type GetInstanceType()
            {
                return typeof(T);
            }

            public object GetInstance()
            {
                if (_instance == null)
                {
                    _instance = _factory.Create();
                    Util.Assert(_instance != null);
                }

                return _instance;
            }
        }

        // NOTE: we need the provider seperate from the creator because
        // if we return the same provider multiple times then the condition 
        // will get over-written
        private class SingletonProvider : ProviderInternal
        {
            ISingletonLazyCreator _creator;

            public SingletonProvider(ISingletonLazyCreator creator)
            {
                _creator = creator;
            }

            public override void OnRemoved()
            {
                _creator.DecRefCount();
            }

            public override Type GetInstanceType()
            {
                return _creator.GetInstanceType();
            }

            public override object GetInstance()
            {
                return _creator.GetInstance();
            }
        }

        private Dictionary<Type, ISingletonLazyCreator> _creators = new Dictionary<Type, ISingletonLazyCreator>();
        private DiContainer _container;

        public SingletonProviderMap(DiContainer container)
        {
            _container = container;
        }

        private void Remove<T>()
        {
            _creators.Remove(typeof(T));
        }

        public ProviderInternal CreateProvider<TConcrete>()
        {
            var type = typeof (TConcrete);

            ISingletonLazyCreator creator;

            if (!_creators.TryGetValue(type, out creator))
            {
                creator = new SingletonLazyCreator<TConcrete>(_container, this);
                _creators.Add(type, creator);
            }

            creator.IncRefCount();

            return new SingletonProvider(creator);
        }
    }
}
