using System;
using UnityEngine;
using System.Collections;
using Zenject;
using Random=UnityEngine.Random;

namespace Asteroids
{
    public class ShipStateDead : ShipState
    {
        Settings _settings;
        float _elapsedTime;
        GameObjectInstantiator _gameObjectCreator;
        GameObject _shipBroken;
		GameObject _shipExplosion;
		
        public ShipStateDead(Settings settings, Ship ship, GameObjectInstantiator gameObjectCreator)
            : base(ship)
        {
            _settings = settings;
            _gameObjectCreator = gameObjectCreator;
        }

        public override void Start()
        {
            _ship.GetComponentInChildren<MeshRenderer>().enabled = false;
			_ship.GetComponentInChildren<ParticleEmitter>().emit = false;
			_ship.audio.Play();
			_shipExplosion = _gameObjectCreator.Instantiate(_settings.explosionTamplate);
			
			 _shipExplosion.transform.position = _ship.Position;
				
            _shipBroken = _gameObjectCreator.Instantiate(_settings.brokenTemplate);

            _shipBroken.transform.position = _ship.Position;
            _shipBroken.transform.rotation = _ship.Rotation;

            foreach (var rigidBody in _shipBroken.GetComponentsInChildren<Rigidbody>())
            {
                var randomTheta = Random.Range(0, Mathf.PI * 2.0f);
                var randomDir = new Vector3(Mathf.Cos(randomTheta), Mathf.Sin(randomTheta), 0);
                rigidBody.AddForce(randomDir * _settings.explosionForce);
            }

            GameEvent.ShipCrashed();
        }

        public override void Stop()
        {
            _ship.GetComponentInChildren<MeshRenderer>().enabled = true;
			_ship.GetComponentInChildren<ParticleEmitter>().emit = true;
            GameObject.Destroy(_shipBroken);
        }

        public override void Update()
        {
        }

        [Serializable]
        public class Settings
        {
            public GameObject brokenTemplate;
			public GameObject explosionTamplate;
            public float explosionForce;
        }
    }
}