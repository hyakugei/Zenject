using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectSingletonProvider<T> : ProviderInternal
    {
        object _instance;
        readonly string _name;
        GameObjectInstantiator _instantiator;

        public GameObjectSingletonProvider(DiContainer container, string name)
        {
            _name = name;
            _instantiator = container.Resolve<GameObjectInstantiator>();
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            if (_instance == null)
            {
                // We don't use the generic version here to avoid duplicate generic arguments to binder
                _instance = _instantiator.Instantiate(typeof(T), _name);
                Assert.That(_instance != null);
            }

            return _instance;
        }
    }
}
