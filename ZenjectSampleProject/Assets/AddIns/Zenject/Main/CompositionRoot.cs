using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public interface IInstaller
    {
        void RegisterBindings(DiContainer container);
    }

    // Define this class as a component of a top-level game object, and define
    // any editor-defined components as a child.  This is necessary to guarantee that
    // UnityContext gets created first before anything that uses IoC
    public class CompositionRoot : MonoBehaviour
    {
        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        void Register()
        {
            var installers = GameObjectUtil.GetChildrenWithInterface<IInstaller>(gameObject);

            Assert.That(installers.Any(), "No installers found");

            foreach (var installer in installers)
            {
                installer.RegisterBindings(_container);
            }
        }

        void InitContainer()
        {
            _container = new DiContainer();
            // Note: This has to go first
            _container.Bind<CompositionRoot>().AsSingle(this);

            // Init default dependencies
            _container.Bind<MonoBehaviourFactory>().AsSingle();

            _container.Bind<UnityEventManager>().AsSingleGameObject();
        }

        void Awake()
        {
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
                    using (new LookupInProgressAdder(_container, component.GetType()))
                    {
                        injecter.Inject(component);
                    }
                }
            }
        }

        void InjectStartingMonoBehaviours()
        {
            // Inject dependencies into child game objects
            foreach (var childTransform in GetComponentsInChildren<Transform>())
            {
                Inject(childTransform.gameObject);
            }

            Debug.Log("CompositionRoot: Finished Injecting Dependencies");
        }

        void Resolve()
        {
            InjectStartingMonoBehaviours();

            _dependencyRoot = _container.Resolve<IDependencyRoot>();

            Assert.That(_dependencyRoot != null, "No dependency root found.");

            _dependencyRoot.Start();
        }
    }
}
