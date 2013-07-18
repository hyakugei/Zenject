using UnityEngine;

namespace Zenject
{
    public class GameObjectFactory
    {
        private IContainer _container;

        public GameObjectFactory(IContainer container)
        {
            _container = container;
        }

        public GameObject Build(string resourceName)
        {
            return Build((GameObject) Resources.Load(resourceName));
        }

        public GameObject Build(string resourceName, Vector3 position, Quaternion rotation)
        {
            return Build((GameObject) Resources.Load(resourceName), position, rotation);
        }

        public GameObject Build(GameObject template)
        {
            return Build(template, Vector3.zero, Quaternion.identity);
        }

        public GameObject Build(GameObject template, Vector3 position, Quaternion rotation)
        {
            var gameObj = (GameObject) Object.Instantiate(template, position, rotation);

            gameObj.SetActive(true);

            var components = gameObj.GetComponentsInChildren<MonoBehaviour>();

            var injecter = new PropertiesInjecter(_container);
            foreach (var t in components)
            {
                ZenUtil.Assert(t != null, "Undefined monobehaviour in template '" + template.name + "'");
                injecter.Inject(t);
            }

            return gameObj;
        }

        public T Build<T>(GameObject template) where T : Component
        {
            var gameObj = Build(template);
            var script = gameObj.GetComponent<T>();
            ZenUtil.Assert(script != null);

            return script;
        }

        public T Build<T>(GameObject template, Vector3 position, Quaternion rotation) where T : Component
        {
            var gameObj = Build(template, position, rotation);
            var script = gameObj.GetComponent<T>();
            ZenUtil.Assert(script != null);

            return script;
        }
    }
}
