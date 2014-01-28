using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        GameOver,
    }

    public class GameController : IEntryPoint, ITickable
    {
        Ship _ship;
        GameState _state = GameState.WaitingToStart;
        AsteroidSpawner _asteroidSpawner;
        float _elapsedTime;

        static bool _hasStarted = false;

        public int TickPriority
        {
            get { return 0; }
        }

        public float ElapsedTime
        {
            get { return _elapsedTime; }
        }

        public GameState State
        {
            get
            {
                return _state;
            }
        }

        public int InitPriority
        {
            get { return (int)EInitPriority.GameController; }
        }

        public GameController(Ship ship, AsteroidSpawner asteroidSpawner)
        {
            _asteroidSpawner = asteroidSpawner;
            _ship = ship;
        }

        public void Initialize()
        {
            Screen.showCursor = false;
            Debug.Log("Started Game");

            GameEvent.ShipCrashed += OnShipCrashed;

            if (_hasStarted)
            {
                StartGame();
            }
        }

        public void Tick()
        {
            switch (_state)
            {
                case GameState.WaitingToStart:
                    UpdateStarting();
                    break;

                case GameState.Playing:
                    UpdatePlaying();
                    break;

                case GameState.GameOver:
                    UpdateGameOver();
                    break;

                default:
                    break;
            }
        }

        void UpdateGameOver()
        {
            Assert.That(_state == GameState.GameOver);

            if (Input.GetMouseButtonDown(0))
            {
                _ship.Position = Vector3.zero;
                _ship.ChangeState(EShipState.Moving);
                _asteroidSpawner.Start();
                _elapsedTime = 0;

                _state = GameState.Playing;
            }
        }

        void OnShipCrashed()
        {
            Assert.That(_state == GameState.Playing);
            _state = GameState.GameOver;
            _asteroidSpawner.Stop();
        }

        void UpdatePlaying()
        {
            Assert.That(_state == GameState.Playing);
            _elapsedTime += Time.deltaTime;
        }

        void UpdateStarting()
        {
            Assert.That(_state == GameState.WaitingToStart);

            if (Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
        }

        void StartGame()
        {
            Assert.That(_state == GameState.WaitingToStart);

            _asteroidSpawner.Start();
            _ship.ChangeState(EShipState.Moving);
            _state = GameState.Playing;
            _hasStarted = true;
        }
    }
}
