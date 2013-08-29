using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectFactory<TContract, TConcrete> : IFactory<TContract>
    {
        GameObject _template;

        public GameObjectFactory(DiContainer container, GameObject template)
        {
            _template = template;
        }

        public TContract Create(params object[] constructorArgs)
        {
            return default(TContract);
        }
    }

    public class GameObjectFactory<TContract> : IFactory<TContract> where TContract : Component
    {
        DiContainer _container;
        GameObject _template;
        GameObjectInstantiator _instantiator;

        public GameObjectFactory(DiContainer container, GameObject template)
        {
            _template = template;
            _container = container;
            _instantiator = _container.Resolve<GameObjectInstantiator>();
        }

        public TContract Create(params object[] constructorArgs)
        {
            var gameObj = _instantiator.Instantiate(_template);

            var component = gameObj.GetComponent<TContract>();
            Util.Assert(component != null, "Could not find component '" + typeof(TContract).Name + "' when creating game object from prefab");

            return component;
        }
    }
}
