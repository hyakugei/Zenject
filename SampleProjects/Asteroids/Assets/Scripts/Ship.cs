using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Zenject;

namespace Asteroids
{
    public class Ship : MonoBehaviour
    {
        public MeshRenderer MeshRenderer;

        [Inject]
        public ShipStateFactory _stateFactory;

        ShipState _state = null;

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return transform.rotation;
            }
            set
            {
                transform.rotation = value;
            }
        }

        public void Start()
        {
            _state = _stateFactory.Create(EShipState.WaitingToStart, this);
        }

        public void Update()
        {
            _state.Update();
        }

        public void OnTriggerEnter(Collider other)
        {
            _state.OnTriggerEnter(other);
        }

        public void ChangeState(EShipState state, params object[] constructorArgs)
        {
            if (_state != null)
            {
                _state.Stop();
            }

            _state = _stateFactory.Create(state, constructorArgs);
            _state.Start();
        }
    }
}
