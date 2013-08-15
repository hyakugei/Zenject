using UnityEngine;
using System.Collections;
using Zenject;

namespace Asteroids
{
    public class SettingsInstaller : MonoBehaviour
    {
        public SettingsWrapper settings;

        public void RegisterBindings(IContainer container)
        {
            container.Bind<ShipStateMoving.Settings>().AsSingle(settings.shipMoving);
            container.Bind<ShipStateDead.Settings>().AsSingle(settings.shipDead);
            container.Bind<ShipStateWaitingToStart.Settings>().AsSingle(settings.shipStarting);

            container.Bind<AsteroidSpawner.Settings>().AsSingle(settings.asteroidSpawner);
        }
    }
}
