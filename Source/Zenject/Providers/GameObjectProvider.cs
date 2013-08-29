using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectSingletonProvider<T> : ProviderInternal
    {
        DiContainer _container;
        object _instance;
        string _name;

        public GameObjectSingletonProvider(DiContainer container, string name)
        {
            _name = name;
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            if (_instance == null)
            {
                var obj = new GameObject(_name);

                // We don't use the generic version here to avoid duplicate generic arguments to binder
                _instance = obj.AddComponent(typeof(T));

                Util.Assert(_instance != null);

                var injecter = new PropertiesInjecter(_container);
                injecter.Inject(_instance);
            }

            return _instance;
        }
    }
}
