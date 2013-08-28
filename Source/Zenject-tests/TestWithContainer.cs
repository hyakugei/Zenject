using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    public class TestWithContainer : TestBase
    {
        protected IContainer _container;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _container = new Container();
        }

        [TearDown]
        public override void Destroy()
        {
            _container = null;
            base.Destroy();
        }
    }

}
