using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    // Define this class as a component of a top-level game object, and define
    // any editor-defined components as a child.  This is necessary to guarantee that
    // UnityContext gets created first before anything that uses IoC
    public class CompositionRoot : MonoBehaviour
    {
        IContainer _container;
        IDependencyRoot _dependencyRoot;

        // Use GameObjectFactory instead of this method when possible
        public void Inject(GameObject gameObj)
        {
            var injecter = new PropertiesInjecter(_container);

            foreach (var component in gameObj.GetComponents<MonoBehaviour>())
            {
                if (component != null && component.enabled) // null if monobehaviour link is broken
                {
                    injecter.Inject(component);
                }
            }
        }

        void Register()
        {
            // use new here instead of at the declaration since otherwise unity will call it before hitting start in the editor
            _container  = new Container();
            BroadcastMessage("RegisterBindings", _container, SendMessageOptions.RequireReceiver);

            // inject dependencies into _all_ components in scene
            foreach (var obj in FindSceneObjectsOfType(typeof (GameObject)))
            {
                Inject((GameObject)obj);
            }

            Debug.Log("CompositionRoot: Finished Injecting Dependencies");
        }

        void Awake()
        {
            Register();
            Resolve();
        }

        void Resolve()
        {
            _dependencyRoot = _container.Resolve<IDependencyRoot>();

            if (_dependencyRoot == null)
            {
                Debug.LogWarning("No dependency root found. Continuing anyway");
            }
        }
    }
}
