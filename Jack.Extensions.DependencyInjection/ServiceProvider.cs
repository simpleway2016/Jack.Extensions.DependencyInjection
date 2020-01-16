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
            _providerSelf = services.BuildServiceProvider();
        }

        static Type FuncBaseType = typeof(System.MulticastDelegate);
        static Type MyType = typeof(ServiceProvider);

        static Dictionary<Type, Delegate> FuncDelegates = new Dictionary<Type, Delegate>();
        static System.Collections.Concurrent.ConcurrentDictionary<Type, object> SingletonInstances = new System.Collections.Concurrent.ConcurrentDictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            var desc = _services.FirstOrDefault(m => m.ServiceType == serviceType);
            if (desc == null)
                return null;

            if (desc.Lifetime == ServiceLifetime.Singleton && SingletonInstances.ContainsKey(serviceType))
                return SingletonInstances[serviceType];


            var obj = _provider?.GetService(serviceType);
            if (obj == null)
                obj = _providerSelf.GetService(serviceType);

            if (obj == null)
                return null;

            if (desc.Lifetime == ServiceLifetime.Singleton)
            {
                SingletonInstances.TryAdd(serviceType, obj);
            }

            var fields = obj.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            foreach( var field in fields )
            {
                if(field.GetCustomAttribute<DependencyInjectionAttribute>() != null)
                {
                    var val = field.GetValue(obj);

                    //if (field.FieldType.BaseType == FuncBaseType && field.FieldType.Name == "Func`1")
                    //{
                    //    
                    //    if (val == null)
                    //    {
                    //        var targetType = field.FieldType.GenericTypeArguments[0];
                    //        if (FuncDelegates.ContainsKey(targetType))
                    //        {
                    //            field.SetValue(obj, FuncDelegates[targetType]);
                    //        }
                    //        else
                    //        {
                    //            var method = MyType.GetMethod("_getobj", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    //            var expression = Expression.Call(Expression.Constant(this), method, Expression.Constant(targetType));
                    //            var expression2 = Expression.Convert(expression, targetType);
                    //            LambdaExpression lambdaExpression = Expression.Lambda(expression2, new ParameterExpression[0]);
                    //            Delegate @delegate = lambdaExpression.Compile();
                    //            FuncDelegates[targetType] = @delegate;

                    //            field.SetValue(obj, @delegate);
                    //        }
                    //    }

                    //}
                    //else

                    if(val == null)
                    {
                        field.SetValue(obj, GetService(field.FieldType));
                    }
                }
            }

            return obj;
        }

        object _getobj(Type targetType)
        {
            return GetService(targetType);
        }

    }

}

