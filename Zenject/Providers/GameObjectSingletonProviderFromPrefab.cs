using System;
using UnityEngine;

namespace Zenject
{
    public class GameObjectSingletonProviderFromPrefab<T> : ProviderInternal where T : Component
    {
        IFactory<T> _factory;
        object _instance;

        public GameObjectSingletonProviderFromPrefab(IContainer container, GameObject template)
        {
            _factory = new GameObjectFactory<T>(container, template);
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            if (_instance == null)
            {
                _instance = _factory.Create();
                ZenUtil.Assert(_instance != null);
            }

            return _instance;
        }
    }
}
