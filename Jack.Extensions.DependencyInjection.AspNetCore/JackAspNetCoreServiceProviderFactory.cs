using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Jack.Extensions.DependencyInjection.AspNetCore
{
    public class JackAspNetCoreServiceProviderFactory : IServiceProviderFactory<Jack.Extensions.DependencyInjection.ServiceProvider>
    {
        public ServiceProvider CreateBuilder(IServiceCollection services)
        {
            services.SupportController();
            return (ServiceProvider)services.BuildJackServiceProvider();
        }

        public IServiceProvider CreateServiceProvider(ServiceProvider containerBuilder)
        {
            return containerBuilder;
        }
    }

}
