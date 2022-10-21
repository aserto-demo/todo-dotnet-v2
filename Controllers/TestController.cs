using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aserto.TodoApp.Controllers
{
    [ApiController]
    [Route("/test")]
    public class TestController : ControllerBase
    {
        [HttpGet()]
        [HttpPost()]
        [HttpPut()]
        [HttpDelete()]
        [Authorize("Aserto")]
        public string Get()
        {
            return "something";
        }
    }
}
