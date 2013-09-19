using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestConditionsParents : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
            public Test0 test0;

            public Test1(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test2
        {
            public Test0 test0;

            public Test2(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test3
        {
            public Test1 test1;

            public Test3(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        class Test4
        {
            public Test1 test1;

            public Test4(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        [Test]
        [ExpectedException]
        public void TestCase1()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Bind<Test0>().AsSingle().When(c => c.parents.Contains(typeof(Test2)));

            _container.Resolve<Test1>();
        }

        [Test]
        public void TestCase2()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Bind<Test0>().AsSingle().When(c => c.parents.Contains(typeof(Test1)));

            var test1 = _container.Resolve<Test1>();
            Assert.That(test1 != null);
        }

        [Test]
        // Test using parents to look deeper up the heirarchy..
            public void TestCase3()
            {
                var t0a = new Test0();
                var t0b = new Test0();

                _container.Bind<Test3>().AsSingle();
                _container.Bind<Test4>().AsSingle();
                _container.Bind<Test1>().AsTransient();

                _container.Bind<Test0>().AsSingle(t0a).When(c => c.parents.Contains(typeof(Test3)));
                _container.Bind<Test0>().AsSingle(t0b).When(c => c.parents.Contains(typeof(Test4)));

                var test3 = _container.Resolve<Test3>();
                var test4 = _container.Resolve<Test4>();

                Assert.That(ReferenceEquals(test3.test1.test0, t0a));
                Assert.That(ReferenceEquals(test4.test1.test0, t0b));
            }
    }
}


