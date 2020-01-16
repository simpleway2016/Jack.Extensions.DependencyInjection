﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
public static class Jack_Extensions_DependencyInjection
{
    /// <summary>
    /// 获取IServiceProvider，支持字段采用[DependencyInjection]方式支持依赖注入，类如果使用[DependencyInjection(Singleton)]等属性，会被自动注入。
    /// 如果有些类是手动注入，那么，请在这些类全部注入后，再调用此方法。
    /// </summary>
    /// <example>
    /// [DependencyInjection]
    /// ICustom _custom;
    /// </example>]
    /// <param name="services"></param>
    /// <param name="serviceProvider">基础的服务提供者</param>
    /// <returns></returns>
    public static IServiceProvider GetJackServiceProvider(this IServiceCollection services, IServiceProvider serviceProvider = null)
    {

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(m => m.IsAbstract == false).ToArray();
            foreach (var type in types)
            {
               var attr =  type.GetCustomAttribute<Jack.Extensions.DependencyInjection.DependencyInjectionAttribute>();
                try
                {
                    if(attr != null)
                    {
                        var registerType = type;
                        if (attr.RegisterType != null)
                            registerType = attr.RegisterType;

                        switch(attr.Mode)
                        {
                            case Jack.Extensions.DependencyInjection.DependencyInjectionMode.Scoped:
                                services.AddScoped(registerType);
                                break;
                            case Jack.Extensions.DependencyInjection.DependencyInjectionMode.Singleton:
                                services.AddSingleton(registerType);
                                break;
                            case Jack.Extensions.DependencyInjection.DependencyInjectionMode.Transient:
                                services.AddTransient(registerType);
                                break;
                        }
                    }
                    
                }
                catch
                {
                }

            }
        }

        return new Jack.Extensions.DependencyInjection.ServiceProvider(services,  serviceProvider);
    }
}