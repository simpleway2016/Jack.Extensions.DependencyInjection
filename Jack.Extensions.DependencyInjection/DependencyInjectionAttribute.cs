using System;
using System.Collections.Generic;
using System.Text;

namespace Jack.Extensions.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class DependencyInjectionAttribute : Attribute
    {
        public DependencyInjectionMode Mode { get; }
        public Type RegisterType { get; internal set; }
        internal System.Reflection.MethodInfo StartupMethod { get; set; }

        public string ExcuMethodOnSingleton { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode">类的注入模式</param>
        /// <param name="registerType">注入为什么类型</param>
        ///  <param name="excuMethodOnSingleton">自动执行的方法名称。如果是Singleton，是否注入后，自动实例化，并执行这个方法。备注：在方法上，加上[StartupOnSingleton]同样可以实现此功能。</param>
        public DependencyInjectionAttribute(DependencyInjectionMode mode = DependencyInjectionMode.Singleton , Type registerType = null, string excuMethodOnSingleton = null)
        {
            this.RegisterType = registerType;
            this.Mode = mode;
            this.ExcuMethodOnSingleton = excuMethodOnSingleton;
        }
    }

    /// <summary>
    /// 标注方法体，在注入后（Singleton类型的注入），自动执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StartupOnSingletonAttribute:Attribute
    {

    }
    public enum DependencyInjectionMode
    {
        Singleton = 1,
        Transient = 2,
        Scoped = 3
    }

}
