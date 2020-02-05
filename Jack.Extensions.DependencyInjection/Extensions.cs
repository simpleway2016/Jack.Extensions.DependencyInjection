﻿using System;
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
    /// <param name="serviceProvider">基础的服务提供者。如果需要同时使用现有的provider，可以传进来，这样，可以保证serviceProvider和本组件的Provider创建的实例是一致的</param>
    /// <returns>返回支持[DependencyInjection]方式的IServiceProvider</returns>
    public static IServiceProvider BuildJackServiceProvider(this IServiceCollection services, IServiceProvider serviceProvider = null)
    {
       var createInstanceTypes = new List<Jack.Extensions.DependencyInjection.DependencyInjectionAttribute>();

        if (!Builded)
        {
            Builded = true;
           
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
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
                                    services.AddSingleton( registerType , type);
                                    if (!string.IsNullOrEmpty(attr.ExcuMethodOnSingleton))
                                    {
                                        attr.RegisterType = registerType;
                                        createInstanceTypes.Add(attr);
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

        var provider = new Jack.Extensions.DependencyInjection.ServiceProvider(services,  serviceProvider);
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

        return provider;
    }
}
