using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AwkwardPresentation.Controllers
{
    public class PresentationController : ApiController
    {
        [HttpGet]
        public string Test()
        {
            return "Hello world";
        }
    }
}