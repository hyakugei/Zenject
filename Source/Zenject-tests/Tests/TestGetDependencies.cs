using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestGetDependencies : TestWithContainer
    {
        class Test5
        {
        }

        class Test4
        {
        }

        class Test3
        {
            public Test3(Test4 test, Test5 test2)
            {
            }
        }

        class Test1
        {
        }

        class Test2
        {
            public Test2(Test1 test1, Test3 test3)
            {
            }
        }

        [Test]
        public void Test()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Bind<Test2>().AsSingle();
            _container.Bind<Test3>().AsSingle();
            _container.Bind<Test4>().AsSingle();
            _container.Bind<Test5>().AsSingle();

            var deps = _container.CalculateObjectGraph<Test2>();

            Assert.That(deps[typeof(Test2)].Count == 2);
            Assert.That(deps[typeof(Test1)].Count == 0);
            Assert.That(deps[typeof(Test3)].Count == 2);
        }
    }
}


