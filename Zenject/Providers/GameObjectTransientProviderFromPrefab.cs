﻿using System;
using UnityEngine;

namespace Zenject
{
    public class GameObjectTransientProviderFromPrefab<T> : ProviderInternal where T : Component
    {
        IFactory<T> _factory;

        public GameObjectTransientProviderFromPrefab(IContainer container, GameObject template)
        {
            _factory = new GameObjectFactory<T>(container, template);
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            return _factory.Create();
        }
    }
}
