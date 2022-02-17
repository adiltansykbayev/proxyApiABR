using Microsoft.AspNetCore.Mvc;

namespace proxyApiABR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpPost(Name = "Index")]
        public List<GoogleModel> Post(string value)
        {
            var list = new List<GoogleModel>();

            return list;
        }
    }
}