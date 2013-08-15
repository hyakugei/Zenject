using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Zenject;
using Random=UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidSpawner : ITickable
    {
        Settings _settings;
        float _timeToNextSpawn;
        float _timeIntervalBetweenSpawns;
        IFactory<Asteroid> _asteroidFactory;
        LevelHelper _level;
        bool _started;
        List<Asteroid> _asteroids = new List<Asteroid>();

        public int TickPriority
        {
            get
            {
                return (int)ETickPriority.AsteroidSpawner;
            }
        }

        public AsteroidSpawner(Settings settings, IFactory<Asteroid> asteroidFactory, LevelHelper level)
        {
            _settings = settings;
            _timeIntervalBetweenSpawns = _settings.maxSpawnTime / (_settings.maxSpawns - _settings.startingSpawns);
            _timeToNextSpawn = _timeIntervalBetweenSpawns;
            _asteroidFactory = asteroidFactory;
            _level = level;
        }

        public void Start()
        {
            Util.Assert(!_started);

            foreach (var asteroid in _asteroids)
            {
                GameObject.Destroy(asteroid.gameObject);
            }

            _asteroids.Clear();

            for (int i = 0; i < _settings.startingSpawns; i++)
            {
                SpawnAsteroid();
            }

            _started = true;
        }

        public void Stop()
        {
            Util.Assert(_started);
            _started = false;
        }

        public void Update()
        {
            if (_started)
            {
                _timeToNextSpawn -= Time.deltaTime;

                if (_timeToNextSpawn < 0 && _asteroids.Count < _settings.maxSpawns)
                {
                    _timeToNextSpawn = _timeIntervalBetweenSpawns;
                    SpawnAsteroid();
                }
            }
        }

        void SpawnAsteroid()
        {
            var asteroid = _asteroidFactory.Create();

            asteroid.Scale = GetRandomScale();
            asteroid.Position = GetRandomStartPosition(asteroid.Scale);
            asteroid.Velocity = GetRandomVelocity();

            _asteroids.Add(asteroid);
        }

        float GetRandomScale()
        {
            return Random.Range(_settings.minScale, _settings.maxScale);
        }

        Vector3 GetRandomVelocity()
        {
            var theta = Random.Range(0, Mathf.PI * 2.0f);
            var dir = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
            var speed = Random.Range(_settings.minSpeed, _settings.maxSpeed);

            return dir * speed;
        }

        Vector3 GetRandomStartPosition(float scale)
        {
            var side = (Side)Random.Range(0, (int)Side.Count);
            var rand = Random.Range(0.0f, 1.0f);

            switch (side)
            {
                case Side.Top:
                    return new Vector3(_level.Left + rand * _level.Width, _level.Top + scale, 0);

                case Side.Bottom:
                    return new Vector3(_level.Left + rand * _level.Width, _level.Bottom - scale, 0);

                case Side.Right:
                    return new Vector3(_level.Right + scale, _level.Bottom + rand * _level.Height, 0);

                case Side.Left:
                    return new Vector3(_level.Left - scale, _level.Bottom + rand * _level.Height, 0);
            }

            Util.Assert(false);
            return Vector3.zero;
        }

        enum Side
        {
            Top,
            Bottom,
            Left,
            Right,
            Count
        }

        [Serializable]
        public class Settings
        {
            public float minSpeed;
            public float maxSpeed;

            public float minScale;
            public float maxScale;

            public int startingSpawns;
            public int maxSpawns;
            public float maxSpawnTime;
        }
    }
}
