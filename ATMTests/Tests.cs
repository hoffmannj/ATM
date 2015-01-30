using ATM;
using NUnit.Framework;
using System.Linq;

namespace ATMTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test_1()
        {
            var m = ATMMain.Create();
            Assert.IsInstanceOf<IATM>(m);
        }

        [Test]
        public void Test_2()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To<TestInterface1Impl1>();
            });
            var o = m.Get<TestInterface1>();
            Assert.IsInstanceOf<TestInterface1Impl1>(o);
        }

        [Test]
        public void Test_3()
        {
            var m = ATMMain.Create();
            var o = m.Get<TestInterface1Impl1>();
            Assert.IsInstanceOf<TestInterface1Impl1>(o);
        }

        [Test]
        public void Test_4()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To<TestInterface1Impl1>();
                mapper.Map<TestInterface2>().To<TestInterface2Impl1>();
            });
            var o = m.Get<TestInterface2>();
            Assert.IsInstanceOf<TestInterface2Impl1>(o);
            Assert.IsInstanceOf<TestInterface1Impl1>(o.GetOtherObject());
        }

        [Test]
        [ExpectedException(typeof(CantResolveDependenciesException))]
        public void Test_5()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface2>().To<TestInterface2Impl1>();
            });
        }

        [Test]
        public void Test_6()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface2>().To<TestInterface2Impl2>();
            });
            var o = m.Get<TestInterface2>();
            Assert.IsInstanceOf<TestInterface2Impl2>(o);
            Assert.IsInstanceOf<TestInterface1Impl1>(o.GetOtherObject());
        }

        [Test]
        public void Test_7()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To<TestInterface1Impl1>();
                mapper.Map<TestInterface2>().To<TestInterface2Impl1>();
                mapper.Map<TestInterface2>().To<TestInterface2Impl2>();
            });
            var o = m.GetAll<TestInterface2>();
            Assert.IsTrue(o.Length == 2);
        }

        [Test]
        public void Test_8()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To<TestInterface1Impl1>();
                mapper.Map<TestInterface2>().To<TestInterface2Impl1>();
                mapper.Map<TestInterface2>().To<TestInterface2Impl2>();
            });
            var o = m.GetAll(typeof(TestInterface2));
            Assert.IsTrue(o.Length == 2);
            Assert.IsTrue(o.All(item => item is TestInterface2));
        }

        [Test]
        public void Test_9()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map(typeof(TestInterface1)).To(typeof(TestInterface1Impl1));
            });
            var o = m.Get(typeof(TestInterface1));
            Assert.IsInstanceOf<TestInterface1Impl1>(o);
        }

        [Test]
        public void Test_10()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface2>().To<TestInterface2Impl1>().WithParameters(new { obj = new TestInterface1Impl1() });
            });
            var o = m.Get<TestInterface2>();
            Assert.IsInstanceOf<TestInterface2Impl1>(o);
            Assert.IsInstanceOf<TestInterface1Impl1>(o.GetOtherObject());
        }

        [Test]
        public void Test_11()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface2>().To<TestInterface2Impl1>().WithParameters(new ParameterObject1 { obj = new TestInterface1Impl1() });
            });
            var o = m.Get<TestInterface2>();
            Assert.IsInstanceOf<TestInterface2Impl1>(o);
            Assert.IsInstanceOf<TestInterface1Impl1>(o.GetOtherObject());
        }

        [Test]
        [ExpectedException(typeof(CantResolveDependenciesException))]
        public void Test_12()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To<TestInterface1Impl2>();
            });
        }

        [Test]
        public void Test_13()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To<TestInterface1Impl1>();
            });
            var o1 = m.Get<TestInterface1>();
            var o2 = m.Get<TestInterface1>();
            Assert.AreNotSame(o1, o2);
        }

        [Test]
        public void Test_14()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To<TestInterface1Impl1>().AsSingleton();
            });
            var o1 = m.Get<TestInterface1>();
            var o2 = m.Get<TestInterface1>();
            Assert.AreSame(o1, o2);
        }

        [Test]
        public void Test_15()
        {
            var m = ATMMain.Create();
            var oo = new TestInterface1Impl1();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To(oo);
            });
            var o = m.Get<TestInterface1>();
            Assert.AreSame(o, oo);
        }

        [Test]
        public void Test_16()
        {
            var m = ATMMain.Create();
            var oo = new TestInterface1Impl1();
            m.Register(mapper =>
            {
                mapper.Map(typeof(TestInterface1)).To(oo);
            });
            var o = m.Get(typeof(TestInterface1));
            Assert.AreSame(o, oo);
        }

        [Test]
        [ExpectedException(typeof(RecursiveDependenciesException))]
        public void Test_17()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To<TestInterface1Impl3>();
                mapper.Map<TestInterface2>().To<TestInterface2Impl1>();
            });
        }

        [Test]
        public void Test_18()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To(() => new TestInterface1Impl1());
            });
            var o = m.Get<TestInterface1>();
            Assert.IsInstanceOf<TestInterface1Impl1>(o);
        }

        [Test]
        public void Test_19()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map(typeof(TestInterface1)).To(() => new TestInterface1Impl1());
            });
            var o = m.Get(typeof(TestInterface1));
            Assert.IsInstanceOf<TestInterface1Impl1>(o);
        }

        [Test]
        [ExpectedException(typeof(WrongNumberOfMappingsException))]
        public void Test_20()
        {
            var m = ATMMain.Create();
            m.Register(mapper =>
            {
                mapper.Map<TestInterface1>().To<TestInterface1Impl1>();
                mapper.Map<TestInterface2>().To<TestInterface2Impl1>();
                mapper.Map<TestInterface2>().To<TestInterface2Impl2>();
            });
            var o = m.Get<TestInterface2>();
        }

        [Test]
        public void Test_21()
        {
            var m = ATMMain.Create();
            var ca = new[] { 'a', 'a', 'a', 'a', 'a' };
            m.Register(mapper =>
            {
                mapper.Map<string>().To<string>().WithParameters(new { c = 'a', count = 5 });
            });
            var o = m.Get<string>();
            Assert.AreEqual("aaaaa", o);
        }
    }
}
