using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        ILogger<WeatherForecastController> _logger { get; set; }


        static int count = 0;
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            count++;
            Debug.WriteLine("count:" + count);
            var rng = new Random();
            var threadid = Thread.CurrentThread.ManagedThreadId;
            if(count < 8)
                Thread.Sleep(60000);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = threadid,
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

    }
}
