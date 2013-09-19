using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestCircularDependencies : TestWithContainer
    {
        class Test1
        {
            [Inject]
            public Test2 test = null;
        }

        class Test2
        {
            [Inject]
            public Test2 test = null;
        }

        [Test]
        [ExpectedException]
        public void Test()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Bind<Test2>().AsSingle();

            var test = _container.Resolve<Test2>();
        }
    }
}


