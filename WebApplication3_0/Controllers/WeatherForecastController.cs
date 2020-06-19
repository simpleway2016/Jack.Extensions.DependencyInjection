using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApplication3_0.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : Controller
    {
        TestObject2 _testObject2 { get; set; }
        TestObject _testObj { get; set; }
        [Jack.Extensions.DependencyInjection.DependencyInjection]
        TestObject obj2;

        SyncContextObject SyncContextObject { get; set; }
        IServiceProvider ServiceProvider { get; set; }
        ILogger<WeatherForecastController> _logger { get; set; }
        List<string> list { get; set; }



        public WeatherForecastController()
        {
            
        }

        [HttpGet]
        public string Get()
        {
            var c1=  ServiceProvider.GetService<SyncContextObject>();
            var c2 = HttpContext.RequestServices.GetService<SyncContextObject>();
            var b = c1 == c2;
            _logger.LogError("test");
            return "ok";
        }

    }
}
