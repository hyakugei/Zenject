using System;
using UnityEngine;

namespace ModestTree.Asteroids
{
    public class SettingsWrapper : MonoBehaviour
    {
        public ShipStateMoving.Settings shipMoving;
        public ShipStateDead.Settings shipDead;
        public ShipStateWaitingToStart.Settings shipStarting;

        public AsteroidSpawner.Settings asteroidSpawner;
        public Asteroid.Settings asteroid;
    }
}
