using Microsoft.AspNetCore.Mvc;

namespace NetCore3Lambda.Controllers
{
    [ApiController]
    [Route("/api/example_endpoint")]
    public class ExampleController : ControllerBase
    {
        [HttpPost]
        public ActionResult<ResponseObject> Get(RequestObject request)
        {
            // ...

            return new ResponseObject();
        }

        public struct RequestObject
        {
            // ...
        }

        public struct ResponseObject
        {
            // ...
        }
    }
}
