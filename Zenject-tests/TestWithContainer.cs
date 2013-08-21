using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    public class TestWithContainer
    {
        protected IContainer _container;

        [SetUp]
        public virtual void Setup()
        {
            _container = new Container();
        }

        [TearDown]
        public virtual void Destroy()
        {
            _container = null;
        }
    }

}
