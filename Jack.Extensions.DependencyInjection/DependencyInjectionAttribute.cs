using System;
using System.Collections.Generic;
using System.Text;

namespace Jack.Extensions.DependencyInjection
{
    public class DependencyInjectionAttribute : Attribute
    {
        public DependencyInjectionMode Mode { get; }
        public Type RegisterType { get; }

        public bool CreateInstanceOnSingleton { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode">类的注入模式</param>
        /// <param name="registerType">注入为什么类型</param>
        ///  <param name="createInstanceOnSingleton">如果是Singleton，是否注入后，自动实例化</param>
        public DependencyInjectionAttribute(DependencyInjectionMode mode = DependencyInjectionMode.Singleton , Type registerType = null, bool createInstanceOnSingleton = false)
        {
            this.RegisterType = registerType;
            this.Mode = mode;
            this.CreateInstanceOnSingleton = createInstanceOnSingleton;
        }
    }

    public enum DependencyInjectionMode
    {
        Singleton = 1,
        Transient = 2,
        Scoped = 3
    }

}
