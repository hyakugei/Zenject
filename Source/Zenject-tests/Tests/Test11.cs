using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class Test11 : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2 : Test1
        {
        }

        private class Test3 : Test1
        {
        }

        private class TestImpl1
        {
            public List<Test1> tests;

            public TestImpl1(List<Test1> tests)
            {
                this.tests = tests;
            }
        }

        private class TestImpl2
        {
            [Inject]
            public List<Test1> tests = null;
        }

        [Test]
        public void TestMultiBind()
        {
            _container.Bind<Test1>().AsSingle<Test2>();
            _container.Bind<Test1>().AsSingle<Test3>();
            _container.Bind<TestImpl1>().AsSingle();

            var test1 = _container.Resolve<TestImpl1>();

            Assert.That(test1.tests.Count == 2);
        }

        [Test]
        public void TestMultiBind2()
        {
            _container.Bind<TestImpl1>().AsSingle();

            var test1 = _container.Resolve<TestImpl1>();

            // Allow for zero
            Assert.That(test1.tests.Count == 0);
        }

        [Test]
        public void TestMultiBindListInjection()
        {
            _container.Bind<Test1>().AsSingle<Test2>();
            _container.Bind<Test1>().AsSingle<Test3>();
            _container.Bind<TestImpl2>().AsSingle();

            var test = _container.Resolve<TestImpl2>();
            Assert.That(test.tests.Count == 2);
        }
    }
}


