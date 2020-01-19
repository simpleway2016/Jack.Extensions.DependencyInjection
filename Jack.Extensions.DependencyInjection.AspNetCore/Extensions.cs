using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Jack.Extensions.DependencyInjection;

public static class Jack_Extensions_DependencyInjection_AspNetCore
{
    static bool Builded = false;
    /// <summary>
    /// 让Jack.Extensions.DependencyInjection支持Controller，此方法应该在BuildJackServiceProvider之前调用
    /// </summary>
    public static void SupportController(this IServiceCollection services)
    {
        

        var oldProvider = services.BuildServiceProvider();
        var oldControllerActivator = oldProvider.GetService<IControllerActivator>();
        if (oldControllerActivator != null)
        {
            services.AddSingleton<ControllerActivatorOriginal>(new ControllerActivatorOriginal() { Original = oldControllerActivator });

            services.Replace(ServiceDescriptor.Transient<IControllerActivator, Jack.Extensions.DependencyInjection.ControllerActivator>());
        }
       
    }
}
