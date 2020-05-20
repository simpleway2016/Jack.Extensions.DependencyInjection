using Jack.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3_0
{
    [DependencyInjection]
    public class TestObject
    {
        public TestObject()
        {

        }

        [StartupOnSingleton]
        void start()
        {

        }
    }

    public class TestObject2
    {

    }
}
