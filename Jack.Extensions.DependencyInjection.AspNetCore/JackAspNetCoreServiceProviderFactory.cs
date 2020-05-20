using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNetCore.Hosting
{
    public class JackAspNetCoreServiceProviderFactory : IServiceProviderFactory<IServiceProvider>
    {
        Assembly[] _scanAssemblies;
        public JackAspNetCoreServiceProviderFactory(params Assembly[] scanAssemblies)
        {
            _scanAssemblies = scanAssemblies;
        }
        public IServiceProvider CreateBuilder(IServiceCollection services)
        {
            services.SupportController();
            return services.BuildJackServiceProvider(_scanAssemblies);
        }

        public IServiceProvider CreateServiceProvider(IServiceProvider containerBuilder)
        {
            return containerBuilder;
        }
    }

}
