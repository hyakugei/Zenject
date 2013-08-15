using UnityEngine;

namespace Zenject
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
                ZenUtil.Assert(t != null, "Undefined monobehaviour in template '" + template.name + "'");
                injecter.Inject(t);
            }

            return gameObj;
        }
    }
}
