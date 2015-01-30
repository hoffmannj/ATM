using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMTests
{
    class TestInterface1Impl2 : TestInterface1
    {
        private int number;
        public TestInterface1Impl2(int num)
        {
            number = num;
        }

        public string GetClassName()
        {
            return "TestInterface1Impl2";
        }
    }
}
