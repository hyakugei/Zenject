using System;
using UnityEngine;

namespace Zenject
{
    public class GameObjectSingletonProviderFromPrefab<T> : ProviderInternal
    {
        private GameObject _template;
        private GameObjectFactory _objFactory;
        private object _instance;

        public GameObjectSingletonProviderFromPrefab(IContainer container, GameObject template)
        {
            _template = template;
            _objFactory = container.Resolve<GameObjectFactory>();
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            if (_instance == null)
            {
                var obj = _objFactory.Build(_template);

                // We don't use the generic version here to avoid duplicate generic arguments to binder
                _instance = obj.GetComponent(typeof(T));

                ZenUtil.Assert(_instance != null);
            }

            return _instance;
        }
    }
}
