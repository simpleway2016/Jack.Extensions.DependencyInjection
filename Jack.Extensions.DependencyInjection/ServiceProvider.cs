using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Jack.Extensions.DependencyInjection
{
    public class ServiceProvider : IServiceProvider
    {
        IServiceCollection _services;
        IServiceProvider _provider;
        IServiceProvider _providerSelf;

        public ServiceProvider(IServiceCollection services,IServiceProvider serviceProvider)
        {
            _services = services;
            _provider = serviceProvider;
            
        }

        public void Init()
        {
            _providerSelf = _services.BuildServiceProvider();
        }

        static Type FuncBaseType = typeof(System.MulticastDelegate);
        static Type MyType = typeof(ServiceProvider);

        static Dictionary<Type, Delegate> FuncDelegates = new Dictionary<Type, Delegate>();
        static List<object> SingletonInstances = new List<object>();

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
                return this;

            return GetService(serviceType,null);
        }

        public object GetService(Type serviceType,Action<object> onCreated)
        {
            var httpContextAccessor = _providerSelf.GetService<IHttpContextAccessor>();
            IServiceProvider curServiceProvider = null;
            if (httpContextAccessor != null && httpContextAccessor.HttpContext != null)
                curServiceProvider = httpContextAccessor.HttpContext.RequestServices;
            if (curServiceProvider == null)
                curServiceProvider = _providerSelf;


            var desc = _services.FirstOrDefault(m => m.ServiceType == serviceType);
            if (desc == null && serviceType.IsGenericType)
            {
                desc = _services.FirstOrDefault(m => m.ServiceType.IsGenericType && serviceType.GetGenericTypeDefinition() == m.ServiceType.GetGenericTypeDefinition());
            }
            if (desc == null)
                return curServiceProvider.GetService(serviceType);



            var obj = _provider?.GetService(serviceType);
            if (obj == null)
                obj = curServiceProvider.GetService(serviceType);

            if (obj == null)
                return null;

            onCreated?.Invoke(obj);

            bool need2add = false;
            if (desc.Lifetime == ServiceLifetime.Singleton && SingletonInstances.Contains(obj) == false)
            {
                need2add = true;
            }
            
            if (need2add)
            {
                lock (SingletonInstances)
                {
                    SingletonInstances.Add(obj);
                }
            }

            if (need2add || desc.Lifetime != ServiceLifetime.Singleton)
            {
                InitFieldsAndProperties(obj);
            }
            return obj;
        }

        public void InitFieldsAndProperties(object obj)
        {
            var fields = obj.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                try
                {
                    if (field.GetCustomAttribute<DependencyInjectionAttribute>() != null)
                    {
                        var val = field.GetValue(obj);
                        if (val == null)
                        {
                            GetService(field.FieldType, (v) =>
                            {
                                field.SetValue(obj, v);
                            });
                           
                        }
                    }
                }
                catch
                {
                }
              
            }

            var pros = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach( var pro in pros )
            {
                var method = pro.GetSetMethod(true);
                if (method == null)
                    continue;

                try
                {
                    if (pro.GetValue(obj) != null)
                        continue;
                }
                catch (Exception)
                {
                    continue;
                }

                GetService(pro.PropertyType,(v)=> {
                    if (method != null)
                    {
                        method.Invoke(obj, new object[] { v });
                    }
                });
            }

            //静态字段
            fields = obj.GetType().GetFields(System.Reflection.BindingFlags.Public | BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            foreach (var field in fields)
            {
                try
                {
                    if (field.GetCustomAttribute<DependencyInjectionAttribute>() != null)
                    {
                        var val = field.GetValue(obj);
                        if (val == null)
                        {
                            GetService(field.FieldType,(v)=> {
                                field.SetValue(null,v);
                            });
                            
                        }
                    }
                }
                catch
                {
                }
            }
        }


    }

}

