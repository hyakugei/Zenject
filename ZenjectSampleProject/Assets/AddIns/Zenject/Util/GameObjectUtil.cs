using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModestTree.Zenject
{
    public static class GameObjectUtil
    {
        public static IEnumerable<T> GetChildrenWithInterface<T>(GameObject gameObj)
        {
            Assert.That(typeof(T).IsInterface);

            return (from c in gameObj.GetComponentsInChildren<MonoBehaviour>() where c.GetType().DerivesFrom<T>() select ((T)(object)c));
        }
    }
}

