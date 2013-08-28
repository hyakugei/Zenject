using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectInstantiator
    {
        IContainer _container;

        public GameObjectInstantiator(IContainer container)
        {
            _container = container;
        }

        public GameObject Instantiate(GameObject template)
        {
            var gameObj = (GameObject)Object.Instantiate(template);
            gameObj.SetActive(true);

            var components = gameObj.GetComponentsInChildren<MonoBehaviour>();

            var injecter = new PropertiesInjecter(_container);
            foreach (var t in components)
            {
                Util.Assert(t != null, "Undefined monobehaviour in template '" + template.name + "'");
                injecter.Inject(t);
            }

            return gameObj;
        }

        public T Instantiate<T>(GameObject template, string name) where T : MonoBehaviour
        {
            var component = Instantiate<T>(template);
            component.gameObject.name = name;
            return component;
        }

        public T Instantiate<T>(GameObject template) where T : MonoBehaviour
        {
            Util.Assert(template != null, "Null template found when instantiating game object");

            var obj = Instantiate(template);

            var component = obj.GetComponent<T>();
            Util.Assert(component != null, "Could not find component with type '" + typeof(T) + "' when instantiating template");

            return component;
        }

        public T Instantiate<T>(string name) where T : MonoBehaviour
        {
            var gameObj = new GameObject(name);
            return gameObj.AddComponent<T>();
        }
    }
}
