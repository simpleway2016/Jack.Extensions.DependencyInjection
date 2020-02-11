using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Jack.Extensions.DependencyInjection;

public static class Jack_Extensions_DependencyInjection
{
    static bool Builded = false;
    /// <summary>
    /// 获取IServiceProvider，支持字段采用[DependencyInjection]方式支持依赖注入，支持属性依赖注入，类如果使用[DependencyInjection(Singleton)]等属性，会被自动注入。
    /// 如果有些类是手动注入，那么，请在这些类全部注入后，再调用此方法。
    /// </summary>
    /// <example>
    /// [DependencyInjection]
    /// ICustom _custom;
    /// </example>]
    /// <param name="services"></param>
    /// <param name="scanAssemblies">指定扫描的程序集，如果不指定，则扫描所有已经加载的程序集</param>
    /// <returns>返回支持[DependencyInjection]方式的IServiceProvider</returns>
    public static IServiceProvider BuildJackServiceProvider(this IServiceCollection services,params Assembly[] scanAssemblies )
    {
        var createInstanceTypes = new List<Jack.Extensions.DependencyInjection.DependencyInjectionAttribute>();
        var createInstanceMethods = new List<Jack.Extensions.DependencyInjection.DependencyInjectionAttribute>();

        if (!Builded)
        {
            Builded = true;

            if(scanAssemblies == null || scanAssemblies.Length == 0)
                scanAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in scanAssemblies)
            {
                var types = assembly.GetTypes().Where(m => m.IsAbstract == false).ToArray();
                foreach (var type in types)
                {
                    var attr = type.GetCustomAttribute<Jack.Extensions.DependencyInjection.DependencyInjectionAttribute>();
                    try
                    {
                        if (attr != null)
                        {
                            var registerType = type;
                            if (attr.RegisterType != null)
                                registerType = attr.RegisterType;

                            switch (attr.Mode)
                            {
                                case Jack.Extensions.DependencyInjection.DependencyInjectionMode.Scoped:
                                    services.AddScoped(registerType, type);
                                    break;
                                case Jack.Extensions.DependencyInjection.DependencyInjectionMode.Singleton:
                                    services.AddSingleton(registerType, type);
                                    if (!string.IsNullOrEmpty(attr.ExcuMethodOnSingleton))
                                    {
                                        attr.RegisterType = registerType;
                                        createInstanceTypes.Add(attr);
                                    }
                                    else
                                    {
                                       var methodinfo =  type.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).Where(m => m.GetCustomAttribute<StartupOnSingletonAttribute>() != null).FirstOrDefault();
                                        if (methodinfo != null)
                                        {
                                            attr.RegisterType = registerType;
                                            attr.StartupMethod = methodinfo;
                                            createInstanceMethods.Add(attr);
                                        }
                                    }
                                    break;
                                case Jack.Extensions.DependencyInjection.DependencyInjectionMode.Transient:
                                    services.AddTransient(registerType, type);
                                    break;
                            }
                        }

                    }
                    catch
                    {
                    }

                }
            }
        }

        var provider = new Jack.Extensions.DependencyInjection.ServiceProvider(services, null);
        services.AddSingleton<Jack.Extensions.DependencyInjection.ServiceProvider>(provider);
        provider.Init();

        foreach (var t in createInstanceTypes)
        {
            var obj = provider.GetService(t.RegisterType);
            var method = obj.GetType().GetMethod(t.ExcuMethodOnSingleton, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
                throw new Exception($"can not find {t.ExcuMethodOnSingleton} in {obj.GetType().FullName}");
            method.Invoke(obj, null);
        }
        foreach (var t in createInstanceMethods)
        {
            var obj = provider.GetService(t.RegisterType);            
            t.StartupMethod.Invoke(obj, null);
        }
        return provider;
    }
}
