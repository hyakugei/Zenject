using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    // Note: this corresponds to the values expected in
    // Input.GetMouseButtonDown() and similar methods
    public enum MouseButtons
    {
        Left,
        Right,
        Middle,
    }

    public class UnityEventManager : MonoBehaviour, ITickable
    {
        public event Action ApplicationGainedFocus = delegate { };
        public event Action ApplicationLostFocus = delegate { };
        public event Action<bool> ApplicationFocusChanged = delegate { };

        public event Action ApplicationQuit = delegate {};
        public event Action Gui = delegate {};
        public event Action DrawGizmos = delegate {};

        public event Action LeftMouseButtonDown = delegate {};
        public event Action LeftMouseButtonUp = delegate {};

        public event Action RightMouseButtonDown = delegate {};
        public event Action RightMouseButtonUp = delegate {};

        public event Action MouseMove = delegate {};

        Vector3 _lastMousePosition;

        [InjectNamed("TickPriority")]
        [InjectOptional]
        int _tickPriority = 0;

        public bool IsFocused
        {
            get;
            private set;
        }

        public int TickPriority
        {
            get
            {
                return _tickPriority;
            }
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown((int)MouseButtons.Left))
            {
                LeftMouseButtonDown();
            }
            else if (Input.GetMouseButtonUp((int)MouseButtons.Left))
            {
                LeftMouseButtonUp();
            }

            if (Input.GetMouseButtonDown((int)MouseButtons.Right))
            {
                RightMouseButtonDown();
            }
            else if (Input.GetMouseButtonUp((int)MouseButtons.Right))
            {
                RightMouseButtonUp();
            }

            if (_lastMousePosition != Input.mousePosition)
            {
                _lastMousePosition = Input.mousePosition;
                MouseMove();
            }
        }

        void OnGUI()
        {
            Gui();
        }

        void OnApplicationQuit()
        {
            ApplicationQuit();
        }

        void OnDrawGizmos()
        {
            DrawGizmos();
        }

        void OnApplicationFocus(bool newIsFocused)
        {
            if (newIsFocused && !IsFocused)
            {
                IsFocused = true;
                ApplicationGainedFocus();
                ApplicationFocusChanged(true);
            }

            if (!newIsFocused && IsFocused)
            {
                IsFocused = false;
                ApplicationLostFocus();
                ApplicationFocusChanged(false);
            }
        }
    }
}
