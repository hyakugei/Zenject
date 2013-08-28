using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Define this class as a component of a top-level game object, and define
    // any editor-defined components as a child.  This is necessary to guarantee that
    // UnityContext gets created first before anything that uses IoC
    public class CompositionRoot : MonoBehaviour
    {
        IContainer _container;
        IDependencyRoot _dependencyRoot;

        void Register()
        {
            // call RegisterBindings on any installers on our game object or somewhere below in the scene heirarhcy
            BroadcastMessage("RegisterBindings", _container, SendMessageOptions.RequireReceiver);
        }

        void InitContainer()
        {
            _container  = new Container();

            // Init default dependencies
            _container.Bind<IKernel>().AsSingleGameObject<UnityKernel>("Kernel");
            _container.Bind<IEntryPoint>().AsSingle<KernelInitializer>();
            _container.Bind<EntryPointInitializer>().AsSingle();
        }

        void Awake()
        {
            // Call into the logging system as early as possible so that it initializes
            Log.Info("Initializing Composition Root");

            InitContainer();
            Register();
            Resolve();
        }

        void Inject(GameObject gameObj)
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

        void InjectStartingMonoBehaviours()
        {
            // inject dependencies into all components in scene
            foreach (var obj in FindSceneObjectsOfType(typeof (GameObject)))
            {
                Inject((GameObject)obj);
            }

            Log.Info("CompositionRoot: Finished Injecting Dependencies");
        }

        void Resolve()
        {
            InjectStartingMonoBehaviours();

            _dependencyRoot = _container.Resolve<IDependencyRoot>();

            if (_dependencyRoot == null)
            {
                Log.Warn("No dependency root found. Continuing anyway");
            }
        }
    }
}
