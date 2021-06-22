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
        Action<IServiceProvider> _buildCallback;

        public JackAspNetCoreServiceProviderFactory()
        {

        }

        public JackAspNetCoreServiceProviderFactory(Action<IServiceProvider> buildCallback)
        {
            this._buildCallback = buildCallback;

        }

        Assembly[] _scanAssemblies;
        public JackAspNetCoreServiceProviderFactory(params Assembly[] scanAssemblies)
        {
            _scanAssemblies = scanAssemblies;
        }
        public IServiceProvider CreateBuilder(IServiceCollection services)
        {
            services.SupportController();
            var provider = services.BuildJackServiceProvider(_scanAssemblies);
            _buildCallback?.Invoke(provider);
            return provider;
        }

        public IServiceProvider CreateServiceProvider(IServiceProvider containerBuilder)
        {
            return containerBuilder;
        }
    }

}
