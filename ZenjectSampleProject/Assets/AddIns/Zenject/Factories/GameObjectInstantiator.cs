using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectInstantiator
    {
        public event Action<GameObject> GameObjectInstantiated = delegate { };
        readonly DiContainer _container;

        CompositionRoot _compRoot;

        public GameObjectInstantiator(DiContainer container)
        {
            _container = container;
            _compRoot = container.Resolve<CompositionRoot>();
        }

        public Transform RootParent
        {
            get
            {
                return _compRoot.transform;
            }
        }

        // Add a monobehaviour to an existing game object
        // Note: gameobject here is not a prefab prototype, it is an instance
        public TContract AddMonobehaviour<TContract>(GameObject gameObject) where TContract : Component
        {
            using (new LookupInProgressAdder(_container, typeof(TContract)))
            {
                var injecter = new PropertiesInjecter(_container);

                var component = gameObject.AddComponent<TContract>();
                injecter.Inject(component);

                return component;
            }
        }

        // Create a new game object from a given prefab 
        // Without returning any particular monobehaviour
        public GameObject Instantiate(GameObject template)
        {
            var gameObj = (GameObject)GameObject.Instantiate(template);

            // By default parent to comp root
            // This is good so that the entire object graph is 
            // contained underneath it, which is useful for cases
            // where you need to delete the entire object graph
            gameObj.transform.parent = _compRoot.transform;

            gameObj.SetActive(true);

            var components = gameObj.GetComponentsInChildren<MonoBehaviour>();

            var injecter = new PropertiesInjecter(_container);
            foreach (var t in components)
            {
                Assert.That(t != null, "Undefined monobehaviour in template '" + template.name + "'");
                injecter.Inject(t);
            }

            GameObjectInstantiated(gameObj);

            return gameObj;
        }

        // Create from prefab and customize name
        // Return specific monobehaviour
        public T Instantiate<T>(GameObject template, string name) where T : MonoBehaviour
        {
            var component = Instantiate<T>(template);
            component.gameObject.name = name;
            return component;
        }

        // Create from prefab
        // Return specific monobehaviour
        public T Instantiate<T>(GameObject template) where T : MonoBehaviour
        {
            Assert.That(template != null, "Null template found when instantiating game object");

            using (new LookupInProgressAdder(_container, typeof(T)))
            {
                var obj = Instantiate(template);

                var component = obj.GetComponent<T>();
                Assert.That(component != null, "Could not find component with type '" + typeof(T) + "' when instantiating template");

                return component;
            }
        }

        public object Instantiate(Type type, string name)
        {
            Assert.That(typeof(Component).IsAssignableFrom(type));

            var gameObj = new GameObject(name);
            gameObj.transform.parent = _compRoot.transform;

            var component = gameObj.AddComponent(type);

            using (new LookupInProgressAdder(_container, type))
            {
                var injecter = new PropertiesInjecter(_container);
                injecter.Inject(component);
            }

            GameObjectInstantiated(gameObj);

            return component;
        }

        public T Instantiate<T>(string name) where T : Component
        {
            return (T)Instantiate(typeof(T), name);
        }

        public GameObject Instantiate(string name)
        {
            var gameObj = new GameObject(name);
            gameObj.transform.parent = _compRoot.transform;

            GameObjectInstantiated(gameObj);

            return gameObj;
        }
    }
}
