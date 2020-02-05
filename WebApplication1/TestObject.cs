using Jack.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    [DependencyInjection(excuMethodOnSingleton:"start")]
    public class TestObject
    {
        public TestObject()
        {

        }

        void start()
        {

        }
    }
}
