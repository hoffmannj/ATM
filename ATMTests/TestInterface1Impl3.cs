using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMTests
{
    class TestInterface1Impl3 : TestInterface1
    {
        private TestInterface2 obj;
        public TestInterface1Impl3(TestInterface2 ti2)
        {
            obj = ti2;
        }

        public string GetClassName()
        {
            return "TestInterface1Impl3";
        }
    }
}
