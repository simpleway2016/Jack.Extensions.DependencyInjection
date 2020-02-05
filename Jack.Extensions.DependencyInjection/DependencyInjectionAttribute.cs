using System;
using System.Collections.Generic;
using System.Text;

namespace Jack.Extensions.DependencyInjection
{
    public class DependencyInjectionAttribute : Attribute
    {
        public DependencyInjectionMode Mode { get; }
        public Type RegisterType { get; internal set; }

        public string ExcuMethodOnSingleton { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode">类的注入模式</param>
        /// <param name="registerType">注入为什么类型</param>
        ///  <param name="excuMethodOnSingleton">自动执行的方法名称。如果是Singleton，是否注入后，自动实例化，并执行这个方法。</param>
        public DependencyInjectionAttribute(DependencyInjectionMode mode = DependencyInjectionMode.Singleton , Type registerType = null, string excuMethodOnSingleton = null)
        {
            this.RegisterType = registerType;
            this.Mode = mode;
            this.ExcuMethodOnSingleton = excuMethodOnSingleton;
        }
    }

    public enum DependencyInjectionMode
    {
        Singleton = 1,
        Transient = 2,
        Scoped = 3
    }

}
