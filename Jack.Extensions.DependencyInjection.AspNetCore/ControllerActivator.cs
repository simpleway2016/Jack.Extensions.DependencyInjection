using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jack.Extensions.DependencyInjection
{
    class ControllerActivator : IControllerActivator
    {
        public readonly ServiceProvider _serviceProvider;
        public readonly ControllerActivatorOriginal _oldDesc;
        public ControllerActivator(ServiceProvider serviceProvider, ControllerActivatorOriginal oldDesc)
        {
            _serviceProvider = serviceProvider;
            _oldDesc = oldDesc;
        }
        public ServiceProvider ServiceProvider;
        public object Create(ControllerContext actionContext)
        {
            var controller = _oldDesc.Original.Create(actionContext);
            _serviceProvider.InitFieldsAndProperties(controller);
            return controller;
        }

        public void Release(ControllerContext context, object controller)
        {
            _oldDesc.Original.Release(context, controller);
        }
    }

    class ControllerActivatorOriginal
    {
        public IControllerActivator Original;
    }
}
