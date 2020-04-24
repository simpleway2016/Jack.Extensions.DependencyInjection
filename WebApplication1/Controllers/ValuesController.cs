using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Jack.Extensions.DependencyInjection;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ValuesController : ControllerBase
    {
        [DependencyInjection]
        static TestObject t3;

        TestObject _t1
        {
            get;
            set;
        }
        [Jack.Extensions.DependencyInjection.DependencyInjection]
        TestObject _t2;
        public ValuesController()
        {
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetName([FromQuery]string key)
        {

            return new object[] { "value1", _t1 == _t2,key };
        }

    }
}
