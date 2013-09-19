using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public class GameInstaller : MonoBehaviour
    {
        public Camera mainCamera;
        public GameObject asteroidTemplate;
        public Ship ship;
        public GuiHandler gui;

        public void RegisterBindings(DiContainer container)
        {
            container.Bind<IDependencyRoot>().AsSingle<GameRoot>();

            container.Bind<Camera>().AsSingle(mainCamera);
            container.Bind<LevelHelper>().AsSingle();

            container.Bind<ITickable>().AsSingle<AsteroidSpawner>();
            container.Bind<AsteroidSpawner>().AsSingle();

            container.Bind<GuiHandler>().AsSingle(gui);

            container.Bind<IFactory<Asteroid>>().AsSingle<GameObjectFactory<Asteroid>>();
            container.Bind<GameObject>().AsSingle(asteroidTemplate).WhenInjectedInto<GameObjectFactory<Asteroid>>();

            container.Bind<IEntryPoint>().AsSingle<GameController>();
            container.Bind<ITickable>().AsSingle<GameController>();
            container.Bind<GameController>().AsSingle();

            container.Bind<Ship>().AsSingle(ship);
            container.Bind<ShipStateFactory>().AsSingle();
        }
    }

    // - The root of the object graph for our main run config
    public class GameRoot : DependencyRootStandard
    {
        [Inject]
        public GuiHandler _guiHandler;
    }
}
