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

            var provider = services.BuildJackServiceProvider();

            var t2 = provider.GetService<Test>();


        }
    }


    [DependencyInjection]
    class a1<T>
    {
    }

    [DependencyInjection]
    class Test
    {
        a1<string> t0 { get; set; }
    }

    [DependencyInjection( DependencyInjectionMode.Singleton , registerType:typeof(Test))]
    class Test2:Test
    {
        [DependencyInjection]
        Test t1;


    }
}
