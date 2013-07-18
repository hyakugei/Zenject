using UnityEngine;

namespace Zenject
{
    public class GameObjectTransientProviderFromPrefab<T> : ProviderInternal
    {
        private GameObject _template;
        private GameObjectFactory _objFactory;

        public GameObjectTransientProviderFromPrefab(IContainer container, GameObject template)
        {
            _template = template;
            _objFactory = container.Resolve<GameObjectFactory>();
        }

        public override object Get()
        {
            var obj = _objFactory.Build(_template);

            // We don't use the generic version here to avoid duplicate generic arguments to binder
            var component = obj.GetComponent(typeof (T));

            ZenUtil.Assert(component != null);
            return component;
        }
    }
}
