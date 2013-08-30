using UnityEngine;

namespace ModestTree.Zenject
{
    // Lazily create singleton providers and ensure that only one exists for every concrete type

    public class Binder<TContract>
    {
        private DiContainer _container;
        private SingletonProviderMap _singletonMap;

        public Binder(DiContainer container, SingletonProviderMap singletonMap)
        {
            _container = container;
            _singletonMap = singletonMap;
        }

        private BindingConditionSetter Bind(ProviderInternal provider)
        {
            _container.RegisterProvider<TContract>(provider);
            return new BindingConditionSetter(provider);
        }

        public BindingConditionSetter AsTransient()
        {
            return Bind(new TransientProvider<TContract>(_container));
        }

        public BindingConditionSetter AsTransient<TConcrete>() where TConcrete : TContract
        {
            return Bind(new TransientProvider<TConcrete>(_container));
        }

        public BindingConditionSetter AsSingle()
        {
            Util.Assert(!typeof(TContract).IsSubclassOf(typeof(MonoBehaviour)), "Should not use AsSingle for Monobehaviours (when binding type " + typeof(TContract).Name + "), you probably want AsSingleFromPrefab or AsSingleGameObject");
            return Bind(_singletonMap.CreateProvider<TContract>());
        }

        public BindingConditionSetter AsSingle<TConcrete>() where TConcrete : TContract
        {
            Util.Assert(!typeof(TConcrete).IsSubclassOf(typeof(MonoBehaviour)), "Should not use AsSingle for Monobehaviours (when binding type "+ typeof(TConcrete).Name +"), you probably want AsSingleFromPrefab or AsSingleGameObject");
            return Bind(_singletonMap.CreateProvider<TConcrete>());
        }

        public BindingConditionSetter AsSingle<TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            Util.Assert(instance != null);
            return Bind(new SingletonInstanceProvider(instance));
        }

        public BindingConditionSetter AsLookup<TConcrete>() where TConcrete : TContract
        {
            return AsMethod(c => c.Resolve<TConcrete>());
        }

        // we can't have this method because of the necessary where() below, so in this case they have to specify TContract twice
        //public BindingConditionSetter AsSingleFromPrefab(GameObject template)

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter AsSingleFromPrefab<TConcrete>(GameObject template) where TConcrete : Component
        {
            Util.Assert(template != null, "Received null template while binding type '" + typeof(TConcrete).Name + "'");
            return Bind(new GameObjectSingletonProviderFromPrefab<TConcrete>(_container, template));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter AsTransientFromPrefab<TConcrete>(GameObject template) where TConcrete : Component
        {
            Util.Assert(template != null);
            return Bind(new GameObjectTransientProviderFromPrefab<TConcrete>(_container, template));
        }

        public BindingConditionSetter AsMethod(MethodProvider<TContract>.Method method)
        {
            return Bind(new MethodProvider<TContract>(method, _container));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter AsSingleGameObject(string name)
        {
            Util.Assert(typeof(TContract).IsSubclassOf(typeof(MonoBehaviour)), "Expected MonoBehaviour derived type when binding type '" + typeof(TContract).Name + "'");
            return Bind(new GameObjectSingletonProvider<TContract>(_container, name));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter AsSingleGameObject<TConcrete>(string name) where TConcrete : MonoBehaviour
        {
            Util.Assert(typeof(TConcrete).IsSubclassOf(typeof(MonoBehaviour)), "Expected MonoBehaviour derived type when binding type '" + typeof(TConcrete).Name + "'");
            return Bind(new GameObjectSingletonProvider<TConcrete>(_container, name));
        }
    }
}
