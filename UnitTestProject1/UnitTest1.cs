using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Jack.Extensions.DependencyInjection;
using System;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ServiceCollection services = new ServiceCollection();


            var provider = services.GetJackServiceProvider();
            var t2 = provider.GetService<Test2>();

           
        }
    }


    [DependencyInjection]
    class a1
    {
    }

    [DependencyInjection]
    class Test
    {
        [DependencyInjection]
        protected Test t0;
    }

    [DependencyInjection( DependencyInjectionMode.Transient)]
    class Test2:Test
    {
        [DependencyInjection]
        Test t1;


    }
}
