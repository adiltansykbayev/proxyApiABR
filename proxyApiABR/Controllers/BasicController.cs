using Microsoft.AspNetCore.Mvc;

namespace proxyApiABR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        [HttpPost(Name = "Search")]
        public List<GoogleModel> Post(string value)
        {
            //TODO: использование HttpClient c Web Proxy
            var list = new List<GoogleModel>();

            return list;
        }
    }
}