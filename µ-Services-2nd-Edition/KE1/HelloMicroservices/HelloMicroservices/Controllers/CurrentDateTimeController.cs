using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelloMicroservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentDateTimeController : ControllerBase
    {
        [HttpGet("/")]
        public object Get() => DateTime.UtcNow;
    }
}