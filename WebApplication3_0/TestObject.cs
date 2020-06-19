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
        public SyncContextObject SyncContextObject { get; set; }
        public TestObject()
        {

        }

        [StartupOnSingleton]
        void start()
        {

        }
    }

    [DependencyInjection(DependencyInjectionMode.Scoped)]
    public class SyncContextObject:IDisposable
    {
        static int totalid = 1;
        static object lockobj = new object();
        public int id;
        public SyncContextObject()
        {
            lock (lockobj)
            {
                id = totalid++;
            }
        }

        TestObject TestObject { get; set; }

        public void Dispose()
        {
            
        }
    }
    [DependencyInjection( DependencyInjectionMode.Transient)]
    public class TestObject2
    {
        public SyncContextObject SyncContextObject { get; set; }
    }
}
