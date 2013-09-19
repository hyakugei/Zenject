using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class Test8 : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test1
        {
            public Test1(Test0 test1)
            {
            }
        }

        [Test]
        [ExpectedException]
        public void TestDuplicateInjection()
        {
            _container.Bind<Test0>().AsSingle();
            _container.Bind<Test0>().AsSingle();

            _container.Bind<Test1>().AsSingle();

            _container.Resolve<Test1>();
        }
    }
}


