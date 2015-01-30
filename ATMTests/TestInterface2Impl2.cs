using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMTests
{
    class TestInterface2Impl2 : TestInterface2
    {
        private object otherObj;

        public TestInterface2Impl2(TestInterface1Impl1 obj)
        {
            otherObj = obj;
        }

        public object GetOtherObject()
        {
            return otherObj;
        }
    }
}
