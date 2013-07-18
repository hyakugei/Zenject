using UnityEngine;

namespace Zenject
{
    public class GameObjectSingletonProvider<T> : ProviderInternal
    {
        Container _container;
        object _instance;
        string _name;

        public GameObjectSingletonProvider(Container container, string name)
        {
            _name = name;
            _container = container;
        }

        public override object Get()
        {
            if (_instance == null)
            {
                var obj = new GameObject(_name);

                // We don't use the generic version here to avoid duplicate generic arguments to binder
                _instance = obj.AddComponent(typeof(T));

                ZenUtil.Assert(_instance != null);

                var injecter = new PropertiesInjecter(_container);
                injecter.Inject(_instance);
            }

            return _instance;
        }
    }
}
