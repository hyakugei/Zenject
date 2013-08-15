using System;
using UnityEngine;
using System.Collections;
using Zenject;

namespace Asteroids
{
    public class ShipStateMoving : ShipState
    {
        Settings _settings;
        Camera _mainCamera;

        public ShipStateMoving(Settings settings, Ship ship, Camera mainCamera)
            : base(ship)
        {
            _settings = settings;
            _mainCamera = mainCamera;
        }

        public override void Update()
        {
            var mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var mousePos = mouseRay.origin;
            mousePos.z = 0;

            var newPosition = Vector3.Lerp(_ship.Position, mousePos, Mathf.Min(1.0f, _settings.moveSpeed * Time.deltaTime));

            var mouseDelta = _ship.Position - newPosition;
            _ship.Position = newPosition;

            var moveDistance = mouseDelta.magnitude;

            if (moveDistance > Mathf.Epsilon)
            {
                var moveDir = mouseDelta / moveDistance;
                _ship.Rotation = Quaternion.LookRotation(moveDir);
            }
        }

        public override void OnTriggerEnter(Collider other)
        {
            Util.Assert(other.tag == "asteroid");
            _ship.ChangeState(EShipState.Dead, _ship);
        }

        [Serializable]
        public class Settings
        {
            public float moveSpeed;
            public float rotateSpeed;
        }
    }
}
