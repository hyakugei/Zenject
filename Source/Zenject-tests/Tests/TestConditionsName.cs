using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestConditionsName : TestWithContainer
    {
        private class Test0
        {

        }

        private class Test1
        {
            public Test1(Test0 name1)
            {
            }
        }

        private class Test2
        {
            public Test2(Test0 name2)
            {
            }
        }

        public override void Setup()
        {
            base.Setup();
            _container.Bind<Test0>().AsSingle().When(r => r.name == "name1");
        }

        [Test]
        [ExpectedException]
        public void TestNameConditionError()
        {
            _container.Bind<Test2>().AsSingle();
            _container.Resolve<Test2>();
        }

        [Test]
        public void TestNameConditionSuccess()
        {
            _container.Bind<Test1>().AsSingle();
            var test1 = _container.Resolve<Test1>();

            Assert.That(test1 != null);
        }
    }
}


