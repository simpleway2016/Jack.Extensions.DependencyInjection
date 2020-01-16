using System;
using System.Collections.Generic;
using System.Text;

namespace Jack.Extensions.DependencyInjection
{
    public class DependencyInjectionAttribute : Attribute
    {
        public DependencyInjectionMode Mode { get; }
        public Type RegisterType { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode">类的注入模式</param>
        /// <param name="registerType">注入为什么类型</param>
        public DependencyInjectionAttribute(DependencyInjectionMode mode = DependencyInjectionMode.Singleton , Type registerType = null)
        {
            this.RegisterType = registerType;
            this.Mode = mode;
        }
    }

    public enum DependencyInjectionMode
    {
        Singleton = 1,
        Transient = 2,
        Scoped = 3
    }

}
